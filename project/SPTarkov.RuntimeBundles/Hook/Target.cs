using NLog.Targets;
using SPTarkov.Common.Utils.Hook;
using SPTarkov.RuntimeBundles;

namespace SPTarkov.RuntimeBundles.Hook
{
	[Target("SPTarkov.RuntimeBundles")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
