﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 视频播放器分P详情视图模型.
/// </summary>
public sealed partial class VideoPlayerPartSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    private readonly Action<VideoPart> _partSelectedAction;

    [ObservableProperty]
    private List<VideoPart> _parts;

    [ObservableProperty]
    private VideoPart _selectedPart;

    [ObservableProperty]
    private bool _onlyIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPartSectionDetailViewModel"/> class.
    /// </summary>
    public VideoPlayerPartSectionDetailViewModel(
        IList<VideoPart> parts,
        string cid,
        Action<VideoPart> action)
    {
        _partSelectedAction = action;
        Parts = parts.ToList();
        SelectedPart = parts.FirstOrDefault(p => p.Identifier.Id == cid);
        OnlyIndex = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsVideoPlayerPartsOnlyIndex, false);
    }

    /// <summary>
    /// 分集变更.
    /// </summary>
    public event EventHandler PartChanged;

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Parts);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    [RelayCommand]
    private void SelectPart(VideoPart part)
    {
        SelectedPart = part;
        _partSelectedAction?.Invoke(part);
        RaisePartChanged();
    }

    [RelayCommand]
    private void UpdateSelectedPart(VideoPart part)
    {
        if (SelectedPart != part)
        {
            SelectedPart = Parts.FirstOrDefault(p => p.Identifier.Id == part.Identifier.Id);
            RaisePartChanged();
        }
    }

    /// <summary>
    /// 发送分集变更事件.
    /// </summary>
    private void RaisePartChanged()
        => PartChanged?.Invoke(this, EventArgs.Empty);

    partial void OnOnlyIndexChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsVideoPlayerPartsOnlyIndex, value);
}
