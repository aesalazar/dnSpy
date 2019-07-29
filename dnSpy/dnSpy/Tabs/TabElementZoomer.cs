/*
    Copyright (C) 2014-2019 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using dnSpy.Contracts.Command;
using dnSpy.Contracts.Controls;
using dnSpy.Contracts.Images;
using dnSpy.Controls;

namespace dnSpy.Tabs {
	sealed class TabElementZoomer : IDisposable {

		readonly List<CommandBinding> commandBindings;
		readonly List<KeyBinding> keyBindings;
		FrameworkElement? zoomElement;
		IUIObjectProvider? uiObjectProvider;
		IZoomable? zoomable;

		public TabElementZoomer() {
			commandBindings = new List<CommandBinding>();
			keyBindings = new List<KeyBinding>();
			ResetBindings();
		}

		void ResetBindings() {
			commandBindings.Clear();
			keyBindings.Clear();

			if (!Commands.CommandService.CommandManager.TryGetKeyInputs(KeyInputGuid, out var keys)) {
				keys = DefaultKeyInputs;
				Commands.CommandService.CommandManager.AddKeyInputs(KeyInputGuid, nameof(TabElementZoomer), keys);
			}

			var name = nameof(ZoomIncrease);
			var cmd = new RoutedCommand(name, typeof(TabElementZoomer));
			commandBindings.Add(new CommandBinding(cmd, (s, e) => ZoomIncrease(), (s, e) => e.CanExecute = true));
			foreach (var input in keys[name])
				keyBindings.Add(new KeyBinding(cmd, input.Key, input.Modifiers));

			name = nameof(ZoomDecrease);
			cmd = new RoutedCommand(name, typeof(TabElementZoomer));
			commandBindings.Add(new CommandBinding(cmd, (s, e) => ZoomDecrease(), (s, e) => e.CanExecute = true));
			foreach (var input in keys[name])
				keyBindings.Add(new KeyBinding(cmd, input.Key, input.Modifiers));

			name = nameof(ZoomReset);
			cmd = new RoutedCommand(name, typeof(TabElementZoomer));
			commandBindings.Add(new CommandBinding(cmd, (s, e) => ZoomReset(), (s, e) => e.CanExecute = true));
			foreach (var input in keys[name])
				keyBindings.Add(new KeyBinding(cmd, input.Key, input.Modifiers));
		}

		public void InstallZoom(IUIObjectProvider provider, FrameworkElement? elem) {
			var zoomable = (provider as IZoomableProvider)?.Zoomable ?? provider as IZoomable;
			if (!(zoomable is null))
				InstallScaleCore(provider, zoomable);
			else
				InstallScaleCore(provider, elem);
		}

		void InstallScaleCore(IUIObjectProvider provider, IZoomable zoomable) {
			UninstallScale();
			if (zoomable is null)
				return;
			this.zoomable = zoomable;
			SetZoomValue(zoomable.ZoomValue, force: true);
		}

		void InstallScaleCore(IUIObjectProvider provider, FrameworkElement? elem) {
			UninstallScale();
			zoomElement = elem;
			uiObjectProvider = provider;
			if (zoomElement is null)
				return;
			zoomElement.PreviewMouseWheel += ZoomElement_PreviewMouseWheel;
			zoomElement.CommandBindings.AddRange(commandBindings);
			zoomElement.InputBindings.AddRange(keyBindings);
			SetZoomValue(ZoomValue, force: true);
		}

		void UninstallScale() {
			zoomable = null;
			uiObjectProvider = null;
			if (zoomElement is null)
				return;
			if (!(metroWindow is null))
				metroWindow.WindowDpiChanged -= MetroWindow_WindowDpiChanged;
			zoomElement.Loaded -= ZoomElement_Loaded;
			zoomElement.PreviewMouseWheel -= ZoomElement_PreviewMouseWheel;
			foreach (var b in commandBindings)
				zoomElement.CommandBindings.Remove(b);
			foreach (var b in keyBindings)
				zoomElement.InputBindings.Remove(b);
			zoomElement = null;
			ResetBindings();
		}

		void ZoomElement_PreviewMouseWheel(object? sender, MouseWheelEventArgs e) {
			if (Keyboard.Modifiers != ModifierKeys.Control)
				return;

			ZoomMouseWheel(e.Delta);
			e.Handled = true;
		}

		void ZoomMouseWheel(int delta) {
			if (delta > 0)
				ZoomIncrease();
			else if (delta < 0)
				ZoomDecrease();
		}

		void ZoomIncrease() => ZoomValue = ZoomSelector.ZoomIn(ZoomValue * 100) / 100;
		void ZoomDecrease() => ZoomValue = ZoomSelector.ZoomOut(ZoomValue * 100) / 100;
		void ZoomReset() => ZoomValue = 1;

		public double ZoomValue {
			get => zoomable?.ZoomValue ?? currentZoomValue;
			set => SetZoomValue(value, force: false);
		}
		double currentZoomValue = 1;
		MetroWindow? metroWindow;

		void SetZoomValue(double value, bool force) {
			var newZoomValue = value;
			if (double.IsNaN(newZoomValue) || Math.Abs(newZoomValue - 1.0) < 0.05)
				newZoomValue = 1.0;

			if (newZoomValue < ZoomSelector.MinZoomLevel / 100)
				newZoomValue = ZoomSelector.MinZoomLevel / 100;
			else if (newZoomValue > ZoomSelector.MaxZoomLevel / 100)
				newZoomValue = ZoomSelector.MaxZoomLevel / 100;

			if (!force && currentZoomValue == newZoomValue)
				return;

			currentZoomValue = newZoomValue;

			if (!(zoomElement is null))
				AddScaleTransform();
		}

		void AddScaleTransform() {
			Debug.Assert(!(zoomElement is null));
			var mwin = GetWindow();
			if (!(mwin is null)) {
				mwin.SetScaleTransform(zoomElement, currentZoomValue);
				DsImage.SetZoom(zoomElement, currentZoomValue);
			}
		}

		MetroWindow? GetWindow() {
			Debug.Assert(!(zoomElement is null));
			if (!(metroWindow is null))
				return metroWindow;
			if (zoomElement is null)
				return null;

			var win = Window.GetWindow(zoomElement);
			metroWindow = win as MetroWindow;
			if (!(metroWindow is null)) {
				metroWindow.WindowDpiChanged += MetroWindow_WindowDpiChanged;
				return metroWindow;
			}

			// zoomElement.IsLoaded can be true if we've moved a tool window, so always hook the
			// loaded event.
			zoomElement.Loaded -= ZoomElement_Loaded;
			zoomElement.Loaded += ZoomElement_Loaded;

			return null;
		}

		void MetroWindow_WindowDpiChanged(object? sender, EventArgs e) {
			Debug.Assert(!(sender is null) && sender == metroWindow);
			((MetroWindow)sender).SetScaleTransform(zoomElement, currentZoomValue);
		}

		void ZoomElement_Loaded(object? sender, RoutedEventArgs e) {
			var fe = (FrameworkElement)sender!;
			fe.Loaded -= ZoomElement_Loaded;
			if (!(zoomElement is null))
				AddScaleTransform();
		}

		public void Dispose() => UninstallScale();

		#region Statics

		private static readonly Guid KeyInputGuid = new Guid("abde0231-6128-4282-afa8-858e1127e25e");

		private static IDictionary<string, IList<KeyInput>> DefaultKeyInputs => new Dictionary<string, IList<KeyInput>> {
			[nameof(ZoomIncrease)] = new[] {KeyInput.Control(Key.OemPlus), KeyInput.Control(Key.Add),},
			[nameof(ZoomDecrease)] = new[] {KeyInput.Control(Key.OemMinus), KeyInput.Control(Key.Subtract),},
			[nameof(ZoomReset)] = new[] {KeyInput.Control(Key.D0), KeyInput.Control(Key.NumPad0),},
		};

		#endregion
	}
}
