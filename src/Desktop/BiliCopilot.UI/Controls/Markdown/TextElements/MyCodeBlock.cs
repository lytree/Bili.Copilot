﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using System.Text;
using Markdig.Syntax;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Text;
using Microsoft.UI;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Markdown.TextElements;

internal class MyCodeBlock : IAddChild
{
    private readonly CodeBlock _codeBlock;
    private Paragraph _paragraph;
    private readonly MarkdownConfig _config;

    public TextElement TextElement => _paragraph;

    public MyCodeBlock(CodeBlock codeBlock, MarkdownConfig config)
    {
        _codeBlock = codeBlock;
        _config = config;
        _paragraph = new Paragraph();
        var container = new InlineUIContainer();
        var grid = new Grid
        {
            Margin = new Thickness(0, 8, 0, 8),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        var richTextBlock = new RichTextBlock();
        richTextBlock.Foreground = config.Themes.CodeBlockForeground;

        var codeHeader = new Grid
        {
            Background = config.Themes.CodeHeaderBackground,
            Padding = new Thickness(8, 6, 8, 6),
            CornerRadius = new CornerRadius(4, 4, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        codeHeader.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        codeHeader.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

        var codeContainer = new Grid
        {
            Background = config.Themes.CodeBlockBackground,
            Padding = _config.Themes.Padding,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(0, 0, 4, 4),
        };

        var headerText = "TEXT";
        if (codeBlock is FencedCodeBlock fencedCodeBlock)
        {
            var formatter = new ColorCode.RichTextBlockFormatter(Extensions.GetOneDarkProStyle());
            var stringBuilder = new StringBuilder();
            headerText = fencedCodeBlock.Info.ToUpperInvariant() ?? "CODE";

            // go through all the lines backwards and only add the lines to a stack if we have encountered the first non-empty line
            var lines = fencedCodeBlock.Lines.Lines;
            var stack = new Stack<string>();
            var encounteredFirstNonEmptyLine = false;
            if (lines != null)
            {
                for (var i = lines.Length - 1; i >= 0; i--)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line.ToString()) && !encounteredFirstNonEmptyLine)
                    {
                        continue;
                    }

                    encounteredFirstNonEmptyLine = true;
                    stack.Push(line.ToString());
                }

                // append all the lines in the stack to the string builder
                while (stack.Count > 0)
                {
                    stringBuilder.AppendLine(stack.Pop());
                }
            }

            formatter.FormatRichTextBlock(stringBuilder.ToString().Trim(), fencedCodeBlock.ToLanguage(), richTextBlock);
        }
        else
        {
            foreach (var line in codeBlock.Lines.Lines)
            {
                var paragraph = new Paragraph();
                var lineString = line.ToString();
                if (!string.IsNullOrWhiteSpace(lineString))
                {
                    paragraph.Inlines.Add(new Run() { Text = lineString });
                }

                richTextBlock.Blocks.Add(paragraph);
            }
        }

        codeContainer.Children.Add(richTextBlock);

        var textBlock = new TextBlock()
        {
            Text = headerText,
            Foreground = config.Themes.CodeBlockForeground,
            IsTextSelectionEnabled = true,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        codeHeader.Children.Add(textBlock);
        Grid.SetColumn(textBlock, 0);

        var btn = new Button()
        {
            Content = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Copy, FontSize = 14 },
            Background = new SolidColorBrush(Colors.Transparent),
            BorderBrush = new SolidColorBrush(Colors.Transparent),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            Width = 28,
            Height = 28,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
        ToolTipService.SetToolTip(btn, ResourceToolkit.GetLocalizedString(StringNames.Copy));
        AutomationProperties.SetName(btn, ResourceToolkit.GetLocalizedString(StringNames.Copy));
        btn.Click += OnCopyButtonClick;
        codeHeader.Children.Add(btn);
        Grid.SetColumn(btn, 1);

        grid.Children.Add(codeHeader);
        grid.Children.Add(codeContainer);
        Grid.SetRow(codeHeader, 0);
        Grid.SetRow(codeContainer, 1);
        container.Child = grid;
        _paragraph.Inlines.Add(container);
    }

    private void OnCopyButtonClick(object sender, RoutedEventArgs e)
    {
        var sb = new StringBuilder();
        foreach (var line in _codeBlock.Lines.Lines)
        {
            sb.AppendLine(line.ToString());
        }

        var dataPackage = new DataPackage();
        dataPackage.SetText(sb.ToString().Trim());
        Clipboard.SetContent(dataPackage);
        GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    public void AddChild(IAddChild child)
    {
    }
}
