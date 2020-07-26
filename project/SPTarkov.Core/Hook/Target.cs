using NLog.Targets;
using SPTarkov.Common.Utils.Hook;

namespace SPTarkov.Core.Hook
{
	[Target("SPTarkov.Core")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
