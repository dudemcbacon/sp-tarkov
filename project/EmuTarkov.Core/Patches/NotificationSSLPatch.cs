using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.Networking;
using HarmonyLib;
using EmuTarkov.Common.Utils.Patching;

namespace EmuTarkov.Core.Patches
{
    public class NotificationSslPatch : GenericPatch<NotificationSslPatch>
    {
        public NotificationSslPatch() : base(transpiler: nameof(PatchTranspiler))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.TargetAssembly
                .GetTypes().Single(x => x.GetMethod("SetUrlParam", BindingFlags.Public | BindingFlags.Instance ) != null)
                .GetNestedTypes(BindingFlags.NonPublic).Single(y => y.GetConstructor(new[] { typeof(int)}) != null)
                .GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        /* Note(PoloYolo): I'm making a big assumption here that no one else is really going to be touching this method (I don't think this
           class warrants any other edits as far as EmuTarkov is concerned), so I'm going to implement this transpiler to go off of code
           index instead of looking for pattern. If, for some reason, someone wants to make further patches to this, let me know?
           
           So, what's going on here?

           GClass1182 has a nested class that is an IEnumerator. In its MoveNext method, there is a particular bit of code that creates
           a UnityWebRequest for a notification poll request. We want this to use a specific certificateHandler that we've already patched
           to always validate, but it uses the default cerficateHandler (set to null) whose validation will fail for EmuTarkov.
           So we patch this in.

           There is this little bit of code in the else statement:
               this.int_0 = -1;
		       this.string_0 = gclass.method_1();
		       this.unityWebRequest_0 = new UnityWebRequest(this.string_0, "GET");
		       this.int_0 = -3;
		       this.unityWebRequest_0.downloadHandler = new DownloadHandlerBuffer();
		       if (gclass.dictionary_1 != null)
		       {
               ...

           We want to add a little initializer to the third line so that it looks like this:
                this.unityWebRequest_0 = new UnityWebRequest(this.string_0, "GET")
			    {
				    certificateHandler = new CertificateHandler()
			    };

            As stated before, this CertificateHandler we've constructed has its ValidateCertificate method patched in SslCertificatePatch,
            so this should allow our notification requests to always be validated.

            For posterity, I'm also including the ILCode transformation here:
            ID  OFF     OpCode  Operand
            124	016F	ldarg.0
            125	0170	ldarg.0
            126	0171	ldfld	string GClass1182/Class1034::string_0
            127	0176	ldstr	"GET"
            128	017B	newobj	instance void [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequest::.ctor(string, string)
           *129	0180	dup
           *130	0181	newobj	instance void [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.CertificateHandler::.ctor()
           *131	0186	callvirt	instance void [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequest::set_certificateHandler(class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.CertificateHandler)
            132	018B	stfld	class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequest GClass1182/Class1034::unityWebRequest_0

            The codes prefixed by the asterisks (*) are the ones patched-in.

        */
        static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            int index = 129;
            CodeInstruction dupCode = new CodeInstruction(OpCodes.Dup);

            Type certificateHandlerType = PatcherConstants.TargetAssembly.GetTypes().Single(x => x.BaseType == typeof(CertificateHandler));
            CodeInstruction newObjCode = new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(certificateHandlerType));
            CodeInstruction callVirtCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(UnityWebRequest), "set_certificateHandler"));

            List<CodeInstruction> insertCodes = new List<CodeInstruction>()
            {
                dupCode,
                newObjCode,
                callVirtCode
            };
            codes.InsertRange(index, insertCodes);

            return codes.AsEnumerable();
        }
    }
}
