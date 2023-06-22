// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using FontShift.Application.UserControls.Primitives;

namespace FontShift.Application.UserControls
{
    [TemplatePart("PART_Background", typeof(Control))]
    public class BackgroundActionPanel : TemplatedControl
    {
        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<MenuStaticItem, ICommand>(nameof(CommandProperty), null);

        public ICommand Command
        {
            get { return GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            Control background = e.NameScope.Find<Control>("PART_Background");
            background.PointerPressed += OnBackgroundPointerPressed;

            base.OnApplyTemplate(e);
        }

        private void OnBackgroundPointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (Command != null)
            {
                if (Command.CanExecute(null))
                {
                    Command.Execute(null);
                }
            }
        }
    }
}
