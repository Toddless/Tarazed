﻿namespace Workout.Planner.Behaviors
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Input;

    public class EventToCommandBehavior : BehaviorBase<View>
    {
        public static readonly BindableProperty EventNameProperty = BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior), null, propertyChanged: OnEventNameChanged);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior), null);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior), null);
        public static readonly BindableProperty InputConverterProperty = BindableProperty.Create(nameof(Converter), typeof(IValueConverter), typeof(EventToCommandBehavior), null);
        public static readonly BindableProperty UseArgsInsteadSenderProperty = BindableProperty.Create(nameof(UserArgsInsteatSender), typeof(bool), typeof(EventToCommandBehavior), null);

        private Delegate? _eventHandler;

        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool UserArgsInsteatSender
        {
            get { return (bool)GetValue(UseArgsInsteadSenderProperty); }
            set { SetValue(UseArgsInsteadSenderProperty, value); }
        }

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(InputConverterProperty); }
            set { SetValue(InputConverterProperty, value); }
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            RegisterEvent(EventName);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);
            DeregisterEvent(EventName);
        }

        private static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EventToCommandBehavior)bindable;
            if (behavior.AssociatedObject == null)
            {
                return;
            }

            string oldEventName = (string)oldValue;
            string newdEventName = (string)newValue;

            behavior.DeregisterEvent(oldEventName);
            behavior.RegisterEvent(newdEventName);
        }

        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            EventInfo eventInfo = AssociatedObject?.GetType().GetRuntimeEvent(eventName) ?? throw new ArgumentNullException(string.Format("EventToCommandBehavior: Can't register the '{0}' event.", EventName));
            MethodInfo? methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod(nameof(OnEvent));
            _eventHandler = methodInfo?.CreateDelegate(eventInfo.EventHandlerType!, this);
            eventInfo.AddEventHandler(AssociatedObject, _eventHandler);
        }

        private void DeregisterEvent(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            if (_eventHandler == null)
            {
                return;
            }

            EventInfo? eventInfo = AssociatedObject?.GetType().GetRuntimeEvent(eventName) ?? throw new ArgumentException(string.Format("EventToCommandBehavior: Can't de-register the '{0}' event.", EventName));
            eventInfo.RemoveEventHandler(AssociatedObject, _eventHandler);
            _eventHandler = null;
        }

        private void OnEvent(object sender, object eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            object? resolverParameter;
            if (UserArgsInsteatSender)
            {
                resolverParameter = sender;
            }
            else
            {
                if (CommandParameter != null)
                {
                    resolverParameter = CommandParameter;
                }
                else if (Converter != null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    // No culture info needed.
                    resolverParameter = Converter.Convert(eventArgs, typeof(object), null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                else
                {
                    resolverParameter = eventArgs;
                }
            }

            if (Command.CanExecute(resolverParameter))
            {
                Command.Execute(resolverParameter);
            }
        }
    }
}
