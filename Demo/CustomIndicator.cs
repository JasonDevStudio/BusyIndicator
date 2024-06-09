using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Demo;

/// <summary>
/// Represents a view model for a custom indicator.
/// </summary>
public partial class CustomIndicator : ObservableObject
{
	private DispatcherTimer dispatcherTimer;
	private TimeSpan elapsedTime;
	
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
	/// <remarks>
	/// The current step indicates the current position or progress of the custom indicator.
	/// This property is an observable property, which means it automatically notifies the UI
	/// when its value changes.
	/// </remarks>
	[ObservableProperty]
	private string currentStep;

	/// <summary>
	/// Represents the progress of a custom indicator.
	/// </summary>
	/// <remarks>
	/// The progress value indicates the current progress of the custom indicator.
	/// </remarks>
	[ObservableProperty]
	private int progress;

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
			if(value)
				this.InitializeTimer();
			else
				this.StopTimer();
		}
	}

	/// <summary>
	/// Represents a view model for a custom indicator.
	/// </summary>
	public AsyncRelayCommand AbortCommand => new(AbortAsync);
	
	public CancellationTokenSource CancellationSource { get; set; }
	
	/// <summary>
	/// Represents a view model for a custom indicator.
	/// </summary>
	public CustomIndicator()
	{
		this.InitializeTimer();
	}
	
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
	/// <remarks>
	/// This method is responsible for stopping the timer used by the CustomIndicatorViewModel class.
	/// </remarks>
	private void StopTimer()
	{
		this.dispatcherTimer.Stop();
	}

	/// <summary>
	/// Handles the Tick event of the DispatcherTimer.
	/// </summary>
	/// <remarks>
	/// This method is executed every time the DispatcherTimer interval is reached.
	/// It is responsible for updating the UI elements such as myLabel's content with the current time in the format "HH:mm:ss".
	/// </remarks>
	/// <param name="sender">The object that raised the event.</param>
	/// <param name="e">The event arguments.</param>
	private void DispatcherTimer_Tick(object sender, EventArgs e)
	{
		elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1)); // 每秒增加1秒
		string formattedTime = $"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
		this.Times = formattedTime; // 更新UI上的标签内容
		this.CurrentStep = $"{formattedTime}"; // 更新UI上的标签内容
	}
	 
	private async Task AbortAsync()
	{
		if(CancellationSource != null)
		{
			CancellationSource?.Cancel();
			await Task.Delay(100);
			CancellationSource?.Dispose();
			CancellationSource = null;
		}
		
		this.IsBusy = false;
	}
}