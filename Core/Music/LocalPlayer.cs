using System;
using System.IO;
using NAudio.Wave;

namespace FreeAMP.Core.Music
{
    public class LocalPlayer : IDisposable
    {
        private AudioFileReader? _audioFile;
        private WaveOutEvent? _outputDevice;

        public bool IsPlaying =>
            _outputDevice?.PlaybackState == PlaybackState.Playing;

        public string? CurrentFile { get; private set; }

        public float Volume
        {
            get => _audioFile?.Volume ?? 1.0f;
            set
            {
                if (_audioFile != null)
                    _audioFile.Volume = Math.Clamp(value, 0f, 1f);
            }
        }

        public TimeSpan Position
        {
            get => _audioFile?.CurrentTime ?? TimeSpan.Zero;
            set
            {
                if (_audioFile != null)
                    _audioFile.CurrentTime = value;
            }
        }

        public TimeSpan Duration =>
            _audioFile?.TotalTime ?? TimeSpan.Zero;

        public void Play(string path)
        {
            if (!File.Exists(path))
                return;

            Stop();

            _audioFile = new AudioFileReader(path);

            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();

            CurrentFile = path;
        }

        public void Pause()
        {
            _outputDevice?.Pause();
        }

        public void Resume()
        {
            _outputDevice?.Play();
        }

        public void TogglePause()
        {
            if (_outputDevice == null)
                return;

            if (_outputDevice.PlaybackState == PlaybackState.Playing)
                _outputDevice.Pause();
            else
                _outputDevice.Play();
        }

        public void Stop()
        {
            _outputDevice?.Stop();

            _audioFile?.Dispose();
            _outputDevice?.Dispose();

            _audioFile = null;
            _outputDevice = null;

            CurrentFile = null;
        }

        public void Seek(double seconds)
        {
            if (_audioFile == null)
                return;

            _audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}