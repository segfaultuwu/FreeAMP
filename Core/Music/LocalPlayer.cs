using System;
using System.IO;
using System.Runtime.Versioning;
using NAudio.Wave;

namespace FreeAMP.Core.Music
{
    [SupportedOSPlatform("windows")]
    public class LocalPlayer : IDisposable
    {
        private WaveStream? _waveStream;
        private WaveOutEvent? _outputDevice;

        public bool IsPlaying =>
            _outputDevice?.PlaybackState == PlaybackState.Playing;

        public string? CurrentFile { get; private set; }

        public float Volume
        {
            get => _outputDevice?.Volume ?? _volume;
            set
            {
                _volume = Math.Clamp(value, 0f, 1f);
                if (_outputDevice != null)
                    _outputDevice.Volume = _volume;
            }
        }
        private float _volume = 1.0f;

        public TimeSpan Position
        {
            get => _waveStream?.CurrentTime ?? TimeSpan.Zero;
            set
            {
                if (_waveStream != null)
                    _waveStream.CurrentTime = value;
            }
        }

        public TimeSpan Duration =>
            _waveStream?.TotalTime ?? TimeSpan.Zero;

        public void Play(string source)
        {
            Stop();

            try
            {
                if (Uri.TryCreate(source, UriKind.Absolute, out Uri? uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    _waveStream = new MediaFoundationReader(source);
                }
                else
                {
                    if (!File.Exists(source))
                        return;
                    _waveStream = new AudioFileReader(source);
                }

                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_waveStream);
                _outputDevice.Volume = Volume;
                _outputDevice.Play();

                CurrentFile = source;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing source: {ex.Message}");
                Stop();
            }
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

            _waveStream?.Dispose();
            _outputDevice?.Dispose();

            _waveStream = null;
            _outputDevice = null;

            CurrentFile = null;
        }

        public void Seek(double seconds)
        {
            if (_waveStream == null)
                return;

            _waveStream.CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}