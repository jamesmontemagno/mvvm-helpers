# MVVM Helpers

Collection of MVVM helper classes for any application.

**Build Status**: [![](https://jamesmontemagno.visualstudio.com/_apis/public/build/definitions/5c11b5bc-b611-475d-a50c-4043a0cbb441/11/badge)](https://dev.azure.com/jamesmontemagno/MvvmHelpers/_build?definitionId=11)

**NuGets**

|Name|Info|
| ------------------- | :------------------: |
|MvvmHelpers|[![NuGet](https://buildstats.info/nuget/Refractored.MvvmHelpers?includePreReleases=true)](https://www.nuget.org/packages/Refractored.MvvmHelpers/)|
|Development Feed|[MyGet](http://myget.org/F/mvvm-helpers)|

## Get Started

Checkout this awesome [video I made](https://channel9.msdn.com/Shows/XamarinShow/The-Xamarin-Show-12-MVVM-Helpers) introducing you to MVVM Helpers :)

## What's included?

I wanted to keep this library really small and just created it for myself, but I hope others find it useful. Here is what I added in and feel free to request new things in the Issues tab.

### ObservableObject
Simple implementation of INotifyPropertyChanged that any class can inherit from.

```csharp
/// <summary>
/// Sets the property.
/// </summary>
/// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
/// <param name="backingStore">Backing store.</param>
/// <param name="value">Value.</param>
/// <param name="propertyName">Property name.</param>
/// <param name="onChanged">On changed.</param>
/// <typeparam name="T">The 1st type parameter.</typeparam>
protected bool SetProperty<T>( ref T backingStore, T value,
    [CallerMemberName]string propertyName = "",
    Action onChanged = null)
```

```csharp
// <summary>
/// Raises the property changed event.
/// </summary>
/// <param name="propertyName">Property name.</param>
protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
```

### BaseViewModel
Your base view model! It implements INotifyPropertyChanged and a bunch of default properties such as **Title, SubTitle, Icon, IsBusy, IsNotBusy, CanLoadMore**.

### Grouping
Best way to group items into a Key/Value pair ObservableCollection for managing groups of data. See my blog post: [Enhancing Xamarin.Forms ListView with Grouping Headers](https://montemagno.com/enhancing-xamarin-forms-listview-with-grouping/)

### ObservableRangeCollection
A super awesome ObservableCollection that adds important methods such as: **AddRange, RemoveRange, Replace, and ReplaceRange**.

### Utils
Any and all extension methods that are nifty. Here are the current ones:

```csharp
/// <summary>
/// Task extension to add a timeout.
/// </summary>
/// <returns>The task with timeout.</returns>
/// <param name="task">Task.</param>
/// <param name="durationMilliseconds">Duration in Milliseconds.</param>
/// <typeparam name="T">The 1st type parameter.</typeparam>
public async static Task<T> WithTimeout<T>(this Task<T> task, int durationMilliseconds)
```

### AsyncCommand, Command, and WeakEventManager

MVVM Helpers now adds in AsyncCommand and Command under the `MvvmHelpers.Commands` namespace! It also has a nice WeakEventManager to help your events be garbage collection safe :)

Code & Inspiration from the following:
* [AsyncCommand blog from John Thiriet](https://johnthiriet.com/mvvm-going-async-with-async-command)
* [AsyncAwaitBestPracties](https://github.com/brminnick/AsyncAwaitBestPractices/) by [@brminnick](https://github.com/brminnick)
* [Xamarin.Forms Command and WeakEventManager](https://github.com/xamarin/Xamarin.Forms)


### Want To Support This Project?
All I have ever asked is to be active by submitting bugs, features, and sending those pull requests down! Want to go further? Make sure to subscribe to my weekly development podcast [Merge Conflict](http://mergeconflict.fm), where I talk all about awesome Xamarin goodies and you can optionally support the show by becoming a [supporter on Patreon](https://www.patreon.com/mergeconflictfm).
