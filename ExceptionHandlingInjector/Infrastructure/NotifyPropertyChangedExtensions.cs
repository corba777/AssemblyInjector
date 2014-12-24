using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace ExceptionHandlingInjector.Infrastructure
{
    public static class NotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<T> OnAnyPropertyChanges<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Select(_ => source);
        }

        public static IObservable<T> OnPropertyChanges<T>(this T source,string propertyName)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler =>  handler.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h).Where(h=>h.EventArgs.PropertyName==propertyName)
                .Select(_ => source);
        }
    }
}