using System.Threading;

namespace Demo;

public interface ICustomIndicator
{
	#region Api

	/// <summary>
	/// Sets the current step of the custom indicator.
	/// </summary>
	/// <param name="step">The step to set.</param>
	void SetStep(string step);

	/// <summary>
	/// Sets the progress of the custom indicator.
	/// </summary>
	/// <param name="progress">The progress value to set.</param>
	void SetProgress(int progress);

	/// <summary>
	/// Displays the custom indicator with an optional step and progress value.
	/// </summary>
	/// <param name="source">The CancellationTokenSource associated with the asynchronous operation.</param>
	/// <param name="canAbort">A boolean value indicating whether the operation can be aborted.</param>
	/// <param name="showStep">A boolean value indicating whether to display the step.</param>
	void Show(CancellationTokenSource source, bool canAbort = true, bool showStep = true);

	/// <summary>
	/// Stops the currently running asynchronous operation and cancels the associated CancellationTokenSource.
	/// </summary>
	void Stop();

	#endregion
}