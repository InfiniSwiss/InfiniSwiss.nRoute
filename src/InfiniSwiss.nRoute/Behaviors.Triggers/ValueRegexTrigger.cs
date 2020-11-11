using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ValueRegexTrigger
        : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(ValueRegexTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        public static readonly DependencyProperty PatternProperty =
            DependencyProperty.Register("Pattern", typeof(string), typeof(ValueRegexTrigger),
            new PropertyMetadata(null));

        private bool _negate;

        #region Properties

        [Category("Common Properties")]
        public string Source
        {
            get { return Convert.ToString(GetValue(SourceProperty)); }
            set { SetValue(SourceProperty, value); }
        }

        [Category("Common Properties")]
        public string Pattern
        {
            get { return Convert.ToString(GetValue(PatternProperty)); }
            set { SetValue(PatternProperty, value); }
        }

        [Category("Common Properties")]
        public bool Negate
        {
            get { return _negate; }
            set { _negate = value; }
        }

        #endregion

        #region Handlers

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueRegexTrigger)d).MatchValue(e.NewValue);
        }

        private void MatchValue(object sourceValue)
        {
            if (this.AssociatedObject == null) return;      // this is to ensure with initial/static set properties the trigger isn't invoked

            if (string.IsNullOrEmpty(Pattern)) return;
            var _matchValue = Convert.ToString(sourceValue);
            var _result = Regex.IsMatch(_matchValue, Pattern);

            if (Negate) _result = !_result;
            if (_result) base.InvokeActions(sourceValue);
        }

        #endregion

    }
}
