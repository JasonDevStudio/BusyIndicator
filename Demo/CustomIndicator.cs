using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;

namespace Demo;

/// <summary>
/// Represents a view model for a custom indicator.
/// </summary>
public partial class CustomIndicator : ObservableObject, ICustomIndicator
{
	#region Fields

	private DispatcherTimer dispatcherTimer;
	private TimeSpan elapsedTime;
	private CancellationTokenSource cancellationSource;

	#endregion

	#region Properties

	/// <summary>
	/// Gets or sets the time value.
	/// </summary>
	/// <remarks>
	/// The time value indicates a specific point in time or a duration.
	/// </remarks>
	[ObservableProperty]
	private string times;

	/// <summary>
	/// Represents the current step of the custom indicator.
	/// </summary> 
	[ObservableProperty]
	private string currentStep;

	/// <summary>
	/// Represents the progress of a custom indicator.
	/// </summary> 
	[ObservableProperty]
	private int progress;

	/// <summary>
	/// Represents a custom indicator view model.
	/// </summary>
	[ObservableProperty]
	private Visibility showStep = Visibility.Collapsed;

	/// <summary>
	/// Represents a view model for a custom indicator.
	/// </summary>
	[ObservableProperty]
	private Visibility canAbort = Visibility.Collapsed;

	/// <summary>
	/// Gets or sets a value indicating whether the view model is busy.
	/// </summary>
	/// <remarks>
	/// When the view model is busy, it indicates that a time-consuming operation is in progress.
	/// </remarks>
	private bool isBusy;

	/// <summary>
	/// Gets or sets a value indicating whether the custom indicator is busy.
	/// </summary>
	/// <remarks>
	/// When IsBusy is set to true, the custom indicator is activated and the timer starts.
	/// The timer ticks every second and updates the UI elements with the current elapsed time.
	/// When IsBusy is set to false, the timer stops and the custom indicator is deactivated.
	/// </remarks>
	public bool IsBusy
	{
		get => this.isBusy;
		set
		{
			this.SetProperty(ref this.isBusy, value);
			if (value)
				this.InitializeTimer();
			else
				this.StopTimer();
		}
	}

	/// <summary>
	/// Represents a view model for a custom indicator.
	/// </summary>
	public AsyncRelayCommand AbortCommand => new(AbortAsync);

	#endregion

	/// <summary>
	/// Represents a view model for a custom indicator.
	/// </summary>
	public CustomIndicator()
	{
		this.InitializeTimer();
	}

	#region Api

	/// <summary>
	/// Sets the current step of the custom indicator.
	/// </summary>
	/// <param name="step">The step to set.</param>
	public void SetStep(string step) => Application.Current.Dispatcher.Invoke(() => this.CurrentStep = step);

	/// <summary>
	/// Sets the progress of the custom indicator.
	/// </summary>
	/// <param name="progress">The progress value to set.</param>
	public void SetProgress(int progress) => Application.Current.Dispatcher.Invoke(() => this.Progress = progress);

	/// <summary>
	/// Displays the custom indicator with an optional step and progress value.
	/// </summary>
	/// <param name="source">The CancellationTokenSource associated with the asynchronous operation.</param>
	/// <param name="canAbort">A boolean value indicating whether the operation can be aborted.</param>
	/// <param name="showStep">A boolean value indicating whether to display the step.</param>
	public void Show(CancellationTokenSource source = default, bool canAbort = true, bool showStep = true)
	{
		this.cancellationSource = source;
		this.IsBusy = true;
		this.CanAbort = canAbort ? Visibility.Visible : Visibility.Collapsed;
		this.ShowStep = showStep ? Visibility.Visible : Visibility.Collapsed;
	}

	/// <summary>
	/// Shows a custom indicator asynchronously.
	/// </summary>
	/// <param name="action">The action to be executed while the indicator is shown.</param>
	/// <param name="source">The cancellation token source to cancel the action and hide the indicator.</param>
	/// <param name="canAbort">Specifies whether the indicator can be aborted.</param>
	/// <param name="showStep">Specifies whether the step of the indicator should be shown.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	public async Task ShowAsync(Func<Task> action, CancellationTokenSource source = default, bool canAbort = true, bool showStep = true)
	{
		try
		{
			this.Show(source, canAbort, showStep);
			await Task.Run(async () => await action());
		}
		finally
		{
			this.Stop();
		}
	}

	/// <summary>
	/// Stops the currently running asynchronous operation and cancels the associated CancellationTokenSource.
	/// </summary>
	public void Stop()
	{
		this.IsBusy = false;
		this.StopTimer();
	}

	#endregion

	#region Methods

	/// <summary>
	/// Initializes the timer.
	/// </summary>
	private void InitializeTimer()
	{
		this.dispatcherTimer = new DispatcherTimer();
		this.dispatcherTimer.Interval = TimeSpan.FromSeconds(1); // 设置时间间隔为1秒
		this.dispatcherTimer.Tick += DispatcherTimer_Tick;
		this.dispatcherTimer.Start();
	}

	/// <summary>
	/// Stops the timer.
	/// </summary> 
	private void StopTimer()
	{
		this.dispatcherTimer.Stop();
		this.elapsedTime = TimeSpan.Zero;
	}

	/// <summary>
	/// Handles the Tick event of the DispatcherTimer.
	/// </summary> 
	/// <param name="sender">The object that raised the event.</param>
	/// <param name="e">The event arguments.</param>
	private void DispatcherTimer_Tick(object sender, EventArgs e)
	{
		elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1)); // 每秒增加1秒 
		this.Times = $"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}"; // 更新UI上的标签内容
		this.CurrentStep = this.Times; // 更新UI上的标签内容
	}

	/// <summary>
	/// Stops the currently running asynchronous operation and cancels the associated CancellationTokenSource.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns> 
	private async Task AbortAsync()
	{
		if (this.cancellationSource != null)
		{
			this.cancellationSource?.Cancel();
			await Task.Delay(100);
			this.cancellationSource?.Dispose();
			this.cancellationSource = null;
		}

		this.Stop();
	}

	#endregion
}