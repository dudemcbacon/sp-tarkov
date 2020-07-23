using NLog.Targets;
using SPTarkov.Common.Utils.Hook;

namespace SPTarkov.Common.Hook
{
	[Target("SPTarkov.Common")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
