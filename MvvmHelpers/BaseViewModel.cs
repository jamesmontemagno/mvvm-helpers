using System;
using System.Threading.Tasks;

namespace MvvmHelpers
{
	/// <summary>
	/// Base view model.
	/// </summary>
	public class BaseViewModel : ObservableObject
	{
		string title = string.Empty;

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get => title;
			set => SetProperty(ref title, value);
		}

		string subtitle = string.Empty;

		/// <summary>
		/// Gets or sets the subtitle.
		/// </summary>
		/// <value>The subtitle.</value>
		public string Subtitle
		{
			get => subtitle;
			set => SetProperty(ref subtitle, value);
		}

		string icon = string.Empty;

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public string Icon
		{
			get => icon;
			set => SetProperty(ref icon, value);
		}

		bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
		public bool IsBusy
		{
			get => isBusy;
			set
			{
				if (SetProperty(ref isBusy, value))
					IsNotBusy = !isBusy;
			}
		}

		bool isNotBusy = true;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is not busy.
		/// </summary>
		/// <value><c>true</c> if this instance is not busy; otherwise, <c>false</c>.</value>
		public bool IsNotBusy
		{
			get => isNotBusy;
			set
			{
				if (SetProperty(ref isNotBusy, value))
					IsBusy = !isNotBusy;
			}
		}

		bool canLoadMore = true;

		/// <summary>
		/// Gets or sets a value indicating whether this instance can load more.
		/// </summary>
		/// <value><c>true</c> if this instance can load more; otherwise, <c>false</c>.</value>
		public bool CanLoadMore
		{
			get => canLoadMore;
			set => SetProperty(ref canLoadMore, value);
		}


		string header = string.Empty;

		/// <summary>
		/// Gets or sets the header.
		/// </summary>
		/// <value>The header.</value>
		public string Header
		{
			get => header;
			set => SetProperty(ref header, value);
		}

		string footer = string.Empty;

		/// <summary>
		/// Gets or sets the footer.
		/// </summary>
		/// <value>The footer.</value>
		public string Footer
		{
			get => footer;
			set => SetProperty(ref footer, value);
		}
		/// <summary>
		/// Perfomce an Task and stops other functions from starting while it's running
		/// If IsBusy=false performce an Task and sets IsBusy=true while the Task is running.
		/// After the Task completes sets IsBusy=false again.	
		/// <para>If IsBusy=true returns.</para>
		/// </summary>	
		/// <example>
		/// Shows how to use this function in combination with a Command
		/// <code>
		/// ButtonSelectionCommand = new AsyncCommand(()=>  DoUITask(() => MoveAsync()));
		/// </code>
		/// </example>
		/// <param name="action">Action</param>
		public void DoUITask(Action action)
		{
			if (IsBusy)
				return;
			IsBusy = true;
			action();
			IsBusy = false;
		}

		/// <summary>
		/// Perfomce an Task and stops other functions from starting while it's running
		/// <para> If IsBusy=false performce an async Task and sets IsBusy=true while the Task is running.
		/// After the Task completes sets IsBusy=false again.</para>	
		/// <para>If IsBusy=true returns.</para>
		/// </summary>	
		/// <example>		 
		/// Shows how to use this function in combination with an async Command
		/// <code>
		/// ButtonSelectionCommand = new AsyncCommand(()=>  DoUITask(new Func &lt; Task &gt; (() => MoveAsync())));
		/// </code>
		/// </example>
		/// <param name="action">Async Func</param>
		/// <returns> Returns an async Task</returns>
		public async Task DoUITask(Func<Task> action)
		{
			if (IsBusy)
				return;
			IsBusy = true;
			await action();
			IsBusy = false;
		}
	}
}


