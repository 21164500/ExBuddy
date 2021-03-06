namespace ExBuddy.Interfaces
{
	using Clio.Utilities;
	using ff14bot.Interfaces;
	using System;
	using System.Threading.Tasks;

	public interface IFlightEnabledPlayerMover : IPlayerMover, IDisposable
	{
		bool CanFly { get; }

		IFlightMovementArgs FlightMovementArgs { get; }

		bool IsLanding { get; }

		bool IsTakingOff { get; }

		Task SetShouldFlyAsync(Task<Func<Vector3, bool>> shouldFlyToFunc);

		bool ShouldFlyTo(Vector3 destination);
        
		bool IsDiving { get; }
	}
}