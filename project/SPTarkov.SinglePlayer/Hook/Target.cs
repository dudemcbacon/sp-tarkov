using NLog.Targets;
using SPTarkov.Common.Utils.Hook;

namespace SPTarkov.SinglePlayer.Hook
{
	[Target("SPTarkov.SinglePlayer")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
