using Godot;
using System;

public partial class AudioManager : Node
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AudioManager();
            }
            return _instance;
        }
    }

    // Plays a button click sound and executes a callback after completion
    public void PlayButtonSound(Node parent, string buttonName, Action callback = null)
    {
        // Duration of button-click sound
        const float soundDuration = 0.34f;
        bool soundPlayed = false;

        // Try to find AudioStreamPlayer2D directly
        var audioPlayer = parent.GetNode<AudioStreamPlayer2D>($"{buttonName}/AudioStreamPlayer2D");

        if (audioPlayer == null)
        {
            // Try using a common AudioStreamPlayer for all buttons
            audioPlayer = parent.GetNode<AudioStreamPlayer2D>("ButtonClickSound");

            // If still null, try to find in parent
            if (audioPlayer == null)
            {
                // Try to find in root
                audioPlayer = parent.GetTree().Root.GetNode<AudioStreamPlayer2D>("ButtonClickSound");
            }
        }

        // Ensure audioPlayer is found and play sound
        if (audioPlayer != null)
        {
            audioPlayer.Play();
            soundPlayed = true;
        }
        else
        {
            // If all methods fail, create AudioStreamPlayer dynamically
            var tempPlayer = new AudioStreamPlayer();
            parent.AddChild(tempPlayer);

            // Try the correct path to sound file
            AudioStream clickSound = null;

            // Try several possible sound file paths
            string[] possiblePaths = new string[] {
                "res://assets/sounds/button-click.mp3",
                "res://assets/sounds/button_click.mp3",
                "res://assets/sounds/button-click.wav",
                "res://assets/sounds/button_click.wav",
            };

            foreach (var path in possiblePaths)
            {
                clickSound = GD.Load<AudioStream>(path);
                if (clickSound != null)
                {
                    GD.Print($"Successfully loaded sound from: {path}");
                    break;
                }
            }

            if (clickSound != null)
            {
                tempPlayer.Stream = clickSound;
                tempPlayer.Play();
                soundPlayed = true;

                // Remove node after playing
                tempPlayer.Finished += () => tempPlayer.QueueFree();
            }
            else
            {
                GD.Print($"Couldn't load button sound file. Paths tried: {string.Join(", ", possiblePaths)}");
                callback?.Invoke();
                return;
            }
        }

        // If sound played successfully and there's a callback, wait before continuing
        if (soundPlayed && callback != null)
        {
            var timer = parent.GetTree().CreateTimer(soundDuration);
            timer.Timeout += () => callback();
        }
    }

    // Plays an object appear/disappear sound
    public void PlayObjectAppearSound(Node parent)
    {
        // Create new AudioStreamPlayer
        var appearSound = new AudioStreamPlayer();
        parent.AddChild(appearSound);

        // Try to load sound
        var sound = GD.Load<AudioStream>("res://assets/sounds/object-appear.mp3");
        if (sound != null)
        {
            appearSound.Stream = sound;
            appearSound.Play();

            // Remove node after playing
            appearSound.Finished += () => appearSound.QueueFree();
        }
        else
        {
            GD.Print("Couldn't load object-appear.mp3 sound");
            appearSound.QueueFree();
        }
    }
}
