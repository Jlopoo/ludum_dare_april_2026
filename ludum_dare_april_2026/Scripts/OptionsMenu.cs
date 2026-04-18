using Godot;
using LudumDareApril2026.Core;

namespace LudumDareApril2026.UI;

public partial class OptionsMenu : Control
{
    private HSlider? _masterVolumeSlider;
    private Button? _backButton;

    public override void _Ready()
    {
        base._Ready();

        _masterVolumeSlider = GetNodeOrNull<HSlider>("CenterContainer/Column/VolumeRow/MasterVolumeSlider");
        _backButton = GetNodeOrNull<Button>("CenterContainer/Column/BackButton");

        if (_masterVolumeSlider is not null)
        {
            _masterVolumeSlider.MinValue = 0;
            _masterVolumeSlider.MaxValue = 1;
            _masterVolumeSlider.Step = 0.01;
            _masterVolumeSlider.Value = AudioManager.Instance.GetMasterVolumeLinear();
            _masterVolumeSlider.ValueChanged += OnMasterVolumeChanged;
        }
        else
        {
            GD.PushError("[OptionsMenu] MasterVolumeSlider not found.");
        }

        if (_backButton is not null)
        {
            _backButton.Pressed += OnBackPressed;
        }
        else
        {
            GD.PushError("[OptionsMenu] BackButton not found.");
        }
    }

    private void OnMasterVolumeChanged(double value)
    {
        AudioManager.Instance.SetMasterVolumeLinear((float)value);
    }

    private void OnBackPressed()
    {
        SceneManager.Instance.GoToMainMenu();
    }
}
