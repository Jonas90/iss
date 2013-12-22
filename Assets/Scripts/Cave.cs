//#define CAVE_WITH_DEBUG_CHANNELS

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Cave
{
	#region AnalogObserver
	/// <summary>
	/// cave::IAnalogObserver aus der C++-Welt in C#. Für das Managen des
	/// nativen Codes wird der AnalogObserverConnector benötigt, siehe
	/// AddAnalogObserver.
	/// </summary>
	/// <typeparam name="T_analog_id">z.B. WiiAnalogId</typeparam>
	/// <typeparam name="T_analog_data">z.B. float</typeparam>
	public interface IAnalogObserver<T_analog_id, T_analog_data>
	{
		void OnAnalogChannelChanged(T_analog_id id, ref T_analog_data value);
		void OnAnalogChanged();
	}

	/// <summary>
	/// Wrapper zwischen nativem und managed Code. Wie das funktioniert, siehe
	/// IButtonObserver.
	/// </summary>
	/// <typeparam name="T_analog_id">z.B. WiiAnalogId</typeparam>
	/// <typeparam name="T_analog_data">z.B. float</typeparam>
	public class AnalogObserverConnector<T_analog_id, T_analog_data>
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void OnAnalogChannelChangedCallBack(T_analog_id id, ref T_analog_data value);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void OnAnalogChangedCallBack();

		public IntPtr NativePointer { get; private set; }
		private IAnalogObserver<T_analog_id, T_analog_data> _impl;

		private OnAnalogChannelChangedCallBack _changedChannelCallBack;
		private OnAnalogChangedCallBack _changedCallBack;
#pragma warning disable 0414
		private object _onChannelChangedBinder;
		private object _onChangedBinder;
#pragma warning restore 0414

		private void OnAnalogChannelChanged(T_analog_id id, ref T_analog_data value)
		{
			_impl.OnAnalogChannelChanged(id, ref value);
		}
		private void OnAnalogChanged()
		{
			_impl.OnAnalogChanged();
		}

		public AnalogObserverConnector(IAnalogObserver<T_analog_id, T_analog_data> impl)
		{
			_impl = impl;
			NativePointer = Marshal.AllocHGlobal(IntPtr.Size * 3);

			IntPtr vtblPtr = PtrHelper.Add(NativePointer, IntPtr.Size);
			Marshal.WriteIntPtr(NativePointer, vtblPtr);

			// Warum diese Methoden hier andersrum stehen als im C++-Interface
			// könnte mal jemand klären...
			_changedCallBack = new OnAnalogChangedCallBack(OnAnalogChanged);
			Marshal.WriteIntPtr(vtblPtr,
				PtrHelper.GetFunctionPointerForDelegate<OnAnalogChangedCallBack>
					(_changedCallBack, out _onChangedBinder));

			_changedChannelCallBack = new OnAnalogChannelChangedCallBack(OnAnalogChannelChanged);
			Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size),
				PtrHelper.GetFunctionPointerForDelegate<OnAnalogChannelChangedCallBack>
					(_changedChannelCallBack, out _onChannelChangedBinder));
		}
	}
	#endregion

	#region ButtonObserver
	/// <summary>
	/// cave::IButtonObserver aus der C++-Welt in C#. Für das Managen des
	/// nativen Codes wird der ButtonObserverConnector benötigt, siehe
	/// AddButtonObserver.
	/// </summary>
	/// <typeparam name="T_button_id">z.B. WiiButtonId</typeparam>
	public interface IButtonObserver<T_button_id>
	{
		void OnButtonDown(T_button_id id);
		void OnButtonUp(T_button_id id);
	}

	/// <summary>
	/// Wrapper zwischen nativem und managed Code. Wie das funktioniert, siehe:
	/// 
	/// http://code4k.blogspot.de/2010/10/implementing-unmanaged-c-interface.html
	/// </summary>
	/// <typeparam name="T_button_id">z.B. WiiButtonId</typeparam>
	public class ButtonObserverConnector<T_button_id>
	{
		// Die Delegates, die später von C++ aufgerufen werden.
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void OnButtonDownCallBack(T_button_id id);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void OnButtonUpCallBack(T_button_id id);
		//[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		//private delegate void OnButtonClickCallBack(T_button_id id);
		//[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		//private delegate void OnButtonDoubleClickCallBack(T_button_id id);

		/// <summary>
		/// Pointer auf das Interface als unamanged Objekt, das wir später
		/// manuell zusammenbauen.
		/// </summary>
		public IntPtr NativePointer { get; private set; }
		private IButtonObserver<T_button_id> __impl;

		// Instanzen der Delegates oben.
		private OnButtonDownCallBack _downCallBack;
		private OnButtonUpCallBack _upCallBack;
		//private onButtonClickCallBack _clickCallBack;
		//private onButtonDoubleClickCallBack _doubleClickCallBack;

#pragma warning disable 0414
		// Objekte, die die Delegates am Leben erhalten (vom Garbage Collector
		// verschonen).
		private object _onButtonDownBinder;
		private object _onButtonUpBinder;
		//private object _onButtonClickBinder;
		//private object _onButtonDoubleClickBinder;
#pragma warning restore 0414

		// Die eigentlichen Callbacks
		private void OnButtonDown(T_button_id id)
		{
			__impl.OnButtonDown(id);
		}
		private void OnButtonUp(T_button_id id)
		{
			__impl.OnButtonUp(id);
		}
		//public abstract void OnButtonClick(T_button_id id);
		//public abstract void OnButtonDoubleClick(T_button_id id);

		public ButtonObserverConnector(IButtonObserver<T_button_id> impl)
		{
			__impl = impl;
			// Wir bauen uns das Interface im Speicher.
			// - 1 Pointer zur Virtual Method Table
			// - danach, weil ein Interface keine Member hat, gleich die Pointer
			//   auf unsere Delegates (1 VTBL + 2 Delegate = 3 Pointer)
			NativePointer = Marshal.AllocHGlobal(IntPtr.Size * 3);

			// Die VTBL zeigt auf die Adresse danach und wird an die erste
			// Stelle geschrieben.
			IntPtr vtblPtr = PtrHelper.Add(NativePointer, IntPtr.Size);
			Marshal.WriteIntPtr(NativePointer, vtblPtr);

			// Nun hintereinander weg die einzelnen Funktionen.
			_downCallBack = new OnButtonDownCallBack(OnButtonDown);
			Marshal.WriteIntPtr(vtblPtr,
				PtrHelper.GetFunctionPointerForDelegate<OnButtonDownCallBack>
					(_downCallBack, out _onButtonDownBinder));

			_upCallBack = new OnButtonUpCallBack(OnButtonUp);
			Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size),
				PtrHelper.GetFunctionPointerForDelegate<OnButtonUpCallBack>
					(_upCallBack, out _onButtonUpBinder));

			//_clickCallBack = new onButtonClickCallBack(onButtonClick);
			//Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size * 2),
			//	PtrHelper.GetFunctionPointerForDelegate<onButtonClickCallBack>
			//		(_clickCallBack, out _onButtonClickBinder));

			//_doubleClickCallBack = new onButtonDoubleClickCallBack(onButtonDoubleClick);
			//Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size * 3),
			//	PtrHelper.GetFunctionPointerForDelegate<onButtonDoubleClickCallBack>
			//		(_doubleClickCallBack, out _onButtonDoubleClickBinder));
		}
	}
	#endregion

	#region TrackerObserver
	/// <summary>
	/// Einfache 4x4-Matrix zur Verwendung mit dem TrackerObserver.
	/// Keine Mathematik, nur Daten.
	/// </summary>
	public class Matrix44f
	{
		/// <summary>
		/// ACHTUNG: Die Matrix ist Column-Major, weil die darauf basierende
		/// C++-Struktur (gmtl::Matrix) es ist! Die Initialisierung unten ist
		/// somit korrekt, aber auch nur, weil die die Transponierung der
		/// Einheitsmatrix wieder die Einheitsmatrix ergibt!
		/// </summary>
		private float[] __data = new float[16] {1.0f, 0.0f, 0.0f, 0.0f,
												0.0f, 1.0f, 0.0f, 0.0f,
												0.0f, 0.0f, 1.0f, 0.0f,
												0.0f, 0.0f, 0.0f, 1.0f};

		/// <summary>
		/// Initialisierung anhand eines Pointers zu einer C++-Datenstruktur,
		/// die ein float-Array darstellt. Hier Pointer auf gmtl::Matrix44f,
		/// da dessen Daten-Array der einzige Member ist und somit an erster
		/// Stelle im Objekt steht.
		/// </summary>
		/// <param name="ptr"></param>
		public Matrix44f(IntPtr ptr)
		{
			Marshal.Copy(ptr, __data, 0, 16);
		}

		/// <summary>
		/// Dran denken: Column-Major!
		/// </summary>
		/// <param name="row">Zeile</param>
		/// <param name="column">Spalte</param>
		/// <returns></returns>
		public float this[int row, int column]
		{
			get
			{
				Debug.Assert((row < 4) && (column < 4));
				return __data[column * 4 + row];
			}
			set
			{
				Debug.Assert((row < 4) && (column < 4));
				__data[column * 4 + row] = value;
			}
		}

		public override string ToString()
		{
			return String.Join(
				", ",
				new List<float>(__data).ConvertAll(i => i.ToString()).ToArray());
		}
	}

	/// <summary>
	/// cave::ITrackerObserver aus der C++-Welt in C#. Für das Managen des
	/// nativen Codes wird der TrackerObserverConnector benötigt, siehe
	/// AddAnalogObserver.
	/// </summary>
	/// <typeparam name="T_tracker_id">z.B. KinectTrackerId</typeparam>
	public interface ITrackerObserver<T_tracker_id>
	{
		void OnTrackerChannelChanged(T_tracker_id id, Matrix44f value);
		void OnTrackerChanged();
	}

	/// <summary>
	/// Wrapper zwischen nativem und managed Code. Wie das funktioniert, siehe
	/// IButtonObserver.
	/// </summary>
	/// <typeparam name="T_tracker_id">z.B. KinectTrackerId</typeparam>
	public class TrackerObserverConnector<T_tracker_id>
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void OnTrackerChannelChangedCallBack(
			T_tracker_id id,
			IntPtr value);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void OnTrackerChangedCallBack();

		public IntPtr NativePointer { get; private set; }
		private ITrackerObserver<T_tracker_id> _impl;

		private OnTrackerChannelChangedCallBack _changedChannelCallBack;
		private OnTrackerChangedCallBack _changedCallBack;

#pragma warning disable 0414
		private object _onChannelChangedBinder;
		private object _onChangedBinder;
#pragma warning restore 0414

		private void OnTrackerChannelChanged(T_tracker_id id, IntPtr value)
		{
			Matrix44f m = new Matrix44f(value);
			_impl.OnTrackerChannelChanged(id, m);
		}

		private void OnTrackerChanged()
		{
			_impl.OnTrackerChanged();
		}

		public TrackerObserverConnector(ITrackerObserver<T_tracker_id> impl)
		{
			_impl = impl;
			NativePointer = Marshal.AllocHGlobal(IntPtr.Size * 3);

			IntPtr vtblPtr = PtrHelper.Add(NativePointer, IntPtr.Size);
			Marshal.WriteIntPtr(NativePointer, vtblPtr);

			// Warum diese Methoden hier andersrum als im C++-Interface stehen
			// müssen, könnte mal jemand klären...
			_changedCallBack = new OnTrackerChangedCallBack(OnTrackerChanged);
			Marshal.WriteIntPtr(vtblPtr,
				PtrHelper.GetFunctionPointerForDelegate<OnTrackerChangedCallBack>
					(_changedCallBack, out _onChangedBinder));

			_changedChannelCallBack = new OnTrackerChannelChangedCallBack(OnTrackerChannelChanged);
			Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size),
				PtrHelper.GetFunctionPointerForDelegate<OnTrackerChannelChangedCallBack>
					(_changedChannelCallBack, out _onChannelChangedBinder));
		}
	}
	#endregion

	#region IAvatarAdapter
	/// <summary>
	/// Einfacher 3-Dimensionaler Vektor zur Verwendung mit dem
	/// IAvatarAdapter. Keine Mathematik, nur Daten.
	/// </summary>
	public class Vec3f : IDisposable
	{
		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __new_vec3f();
		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __destruct_vec3f(IntPtr ptr);

		private IntPtr __nativePtr = IntPtr.Zero;

		public float[] __data = new float[3] { 0.0f, 0.0f, 0.0f };

		public Vec3f(IntPtr ptr)
		{
			Marshal.Copy(ptr, __data, 0, 3);
		}

		public Vec3f(float x, float y, float z)
		{
			__data[0] = x;
			__data[1] = y;
			__data[2] = z;
		}

		public float this[int i]
		{
			get
			{
				Debug.Assert(i < 4);
				return __data[i];
			}
			set
			{
				Debug.Assert(i < 4);
				__data[i] = value;
			}
		}

		public IntPtr NativePtr()
		{
			if (__nativePtr == IntPtr.Zero)
			{
				__nativePtr = __new_vec3f();
				Marshal.Copy(__data, 0, __nativePtr, 3);
			}
			return __nativePtr;
		}

		public void Dispose()
		{
			if (__nativePtr != IntPtr.Zero)
			{
				__destruct_vec3f(__nativePtr);
			}
		}

		public override string ToString()
		{
			return String.Join(
				", ",
				new List<float>(__data).ConvertAll(i => i.ToString()).ToArray());
		}
	}

	/// <summary>
	/// cave::IAvatarAdapter aus der C++-Welt in C#. Für das Managen
	/// des nativen Codes wird der AvatarAdapterConnector benötigt,
	/// siehe AddButtonObserver.
	/// </summary>
	public interface IAvatarAdapter
	{
		void setTargetVelocity(Vec3f vel);
		void setTargetTurnVelocity(Vec3f vel);
		Vec3f getVelocity();
		Vec3f getTurnVelocity();
	}

	/// <summary>
	///  Wrapper zwischen nativem und managed Code. Wie das funktioniert, siehe:
	/// 
	/// http://code4k.blogspot.de/2010/10/implementing-unmanaged-c-interface.html
	/// </summary>
	public class AvatarAdapterConnector
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void setTargetVelocityCallBack(IntPtr vel);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void setTargetTurnVelocityCallBack(IntPtr vel);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr getVelocityCallBack();
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr getTurnVelocityCallBack();

		public IntPtr NativePointer { get; private set; }
		private IAvatarAdapter _impl;

		private setTargetVelocityCallBack _setTargetVelocityCallBack;
		private setTargetTurnVelocityCallBack _setTargetTurnVelocityCallBack;
		private getVelocityCallBack _getVelocityCallBack;
		private getTurnVelocityCallBack _getTurnVelocityCallBack;
#pragma warning disable 0414
		private object _setTargetVelocityCallBackBinder;
		private object _setTargetTurnVelocityCallBackBinder;
		private object _getVelocityCallBackBinder;
		private object _getTurnVelocityCallBackBinder;
#pragma warning restore 0414

		private void setTargetVelocity(IntPtr vel)
		{
			_impl.setTargetVelocity(new Vec3f(vel));
		}

		private void setTargetTurnVelocity(IntPtr vel)
		{
			_impl.setTargetTurnVelocity(new Vec3f(vel));
		}

		private IntPtr getVelocity()
		{
			return _impl.getVelocity().NativePtr();
		}

		private IntPtr getTurnVelocity()
		{
			return _impl.getTurnVelocity().NativePtr();
		}

		public AvatarAdapterConnector(IAvatarAdapter impl)
		{
			_impl = impl;
			NativePointer = Marshal.AllocHGlobal(IntPtr.Size * 5);

			IntPtr vtblPtr = PtrHelper.Add(NativePointer, IntPtr.Size);
			Marshal.WriteIntPtr(NativePointer, vtblPtr);

			_setTargetVelocityCallBack = new setTargetVelocityCallBack(setTargetVelocity);
			Marshal.WriteIntPtr(vtblPtr,
				PtrHelper.GetFunctionPointerForDelegate<setTargetVelocityCallBack>
					(_setTargetVelocityCallBack, out _setTargetVelocityCallBackBinder));

			_setTargetTurnVelocityCallBack = new setTargetTurnVelocityCallBack(setTargetTurnVelocity);
			Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size),
				PtrHelper.GetFunctionPointerForDelegate<setTargetTurnVelocityCallBack>
					(_setTargetTurnVelocityCallBack, out _setTargetTurnVelocityCallBackBinder));

			_getVelocityCallBack = new getVelocityCallBack(getVelocity);
			Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size * 2),
				PtrHelper.GetFunctionPointerForDelegate<getVelocityCallBack>
					(_getVelocityCallBack, out _getVelocityCallBackBinder));

			_getTurnVelocityCallBack = new getTurnVelocityCallBack(getTurnVelocity);
			Marshal.WriteIntPtr(PtrHelper.Add(vtblPtr, IntPtr.Size * 3),
				PtrHelper.GetFunctionPointerForDelegate<getTurnVelocityCallBack>
					(_getTurnVelocityCallBack, out _getTurnVelocityCallBackBinder));
		}
	}
	#endregion

	#region Button Defs
	/// <summary>
	/// Kopie von ButtonState in C++ (AbstractButtonDevice.h)
	/// </summary>
	public enum ButtonState { UP, TOGGLE_UP, DOWN, TOGGLE_DOWN, UNDEF };

	/// <summary>
	/// Kleine Helfer, um ButtonStates zu interpretieren
	/// </summary>
	public class ButtonHelper
	{
		/// <summary>
		/// Liefert True, wenn der Button unten ist (gerade oder schon länger)
		/// </summary>
		/// <param name="bs">zu interpretierender State</param>
		/// <returns>True, wenn Button unten</returns>
		public static bool isDownOrToggle(ButtonState bs)
		{
			return bs == ButtonState.DOWN || bs == ButtonState.TOGGLE_DOWN;
		}

		/// <summary>
		/// Liefert True, wenn der Button oben ist (gerade oder schon länger)
		/// </summary>
		/// <param name="bs">zu interpretierender State</param>
		/// <returns>True, wenn Button oben</returns>
		public static bool isUpOrToggle(ButtonState bs)
		{
			return bs == ButtonState.UP || bs == ButtonState.TOGGLE_UP;
		}
	}
	#endregion

	#region Devices
	public class WiiMote : IDisposable
	{
		#region Wii Element Enums
		public enum ButtonId
		{
			A = 0, B,
			C, Z,
			ONE, TWO,
			PLUS, MINUS,
			HOME,
			UP, DOWN, RIGHT, LEFT,
			STICK_DIGITAL_UP, STICK_DIGITAL_DOWN,
			STICK_DIGITAL_LEFT, STICK_DIGITAL_RIGHT
		};

		public enum AnalogId
		{
			ACC_X = 0, ACC_Y, ACC_Z,
			ACC_PITCH, ACC_ROLL,
			STICK_X, STICK_Y,
			NUNCHUK_ACC_PITCH, NUNCHUK_ACC_ROLL,
			NUNCHUK_ACC_X, NUNCHUK_ACC_Y, NUNCHUK_ACC_Z
		};
		#endregion

		#region Wii DLLImports
		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr __construct([MarshalAs(UnmanagedType.LPStr)]string address);

		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void __destruct(IntPtr self);

		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void __update(IntPtr self);

		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern ButtonState __getButtonState(IntPtr self, WiiMote.ButtonId id);

		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern float __getAnalogValue(IntPtr self, WiiMote.AnalogId id);

		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void __addButtonObserver(IntPtr self, IntPtr observer);

		[DllImport("WiiClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void __addAnalogObserver(IntPtr self, IntPtr observer);
		#endregion

		public IntPtr NativePointer { get; private set; }

		public WiiMote(string address)
		{
			NativePointer = __construct(address);
		}

		public void Dispose()
		{
			__destruct(NativePointer);
		}

		#region Wii Wrapper Methods
		public void update()
		{
			__update(NativePointer);
		}

		public ButtonState getButtonState(WiiMote.ButtonId id)
		{
			return __getButtonState(NativePointer, id);
		}

		public float getAnalogValue(WiiMote.AnalogId id)
		{
			return __getAnalogValue(NativePointer, id);
		}

		public void addButtonObserver(ButtonObserverConnector<WiiMote.ButtonId> observer)
		{
			__addButtonObserver(NativePointer, observer.NativePointer);
		}

		public void addAnalogObserver(AnalogObserverConnector<WiiMote.AnalogId, float> observer)
		{
			__addAnalogObserver(NativePointer, observer.NativePointer);
		}
		#endregion
	};

	public class Kinect : IDisposable
	{
		#region Kinect Element Enums
		public enum TrackerId
		{
			HIP_CENTER = 0, SPINE, SHOULDER_CENTER, HEAD,
			SHOULDER_LEFT, ELBOW_LEFT, WRIST_LEFT, HAND_LEFT,
			SHOULDER_RIGHT, ELBOW_RIGHT, WRIST_RIGHT, HAND_RIGHT,
			HIP_LEFT, KNEE_LEFT, ANKLE_LEFT, FOOT_LEFT,
			HIP_RIGHT, KNEE_RIGHT, ANKLE_RIGHT, FOOT_RIGHT,
			RECOGNIZED_LEFT_HAND_CENTER, RECOGNIZED_LEFT_HAND_TIP,
			RECOGNIZED_RIGHT_HAND_CENTER, RECOGNIZED_RIGHT_HAND_TIP,
#if CAVE_WITH_DEBUG_CHANNELS
			KINECT_DBG_TRACKER_0,  KINECT_DBG_TRACKER_1,   KINECT_DBG_TRACKER_2,
			KINECT_DBG_TRACKER_3,  KINECT_DBG_TRACKER_4,   KINECT_DBG_TRACKER_5,
			KINECT_DBG_TRACKER_6,  KINECT_DBG_TRACKER_7,   KINECT_DBG_TRACKER_8,
			KINECT_DBG_TRACKER_9,  KINECT_DBG_TRACKER_10,  KINECT_DBG_TRACKER_11,
			KINECT_DBG_TRACKER_12, KINECT_DBG_TRACKER_13,  KINECT_DBG_TRACKER_14,
			KINECT_DBG_TRACKER_15, KINECT_DBG_TRACKER_16,  KINECT_DBG_TRACKER_17,
			KINECT_DBG_TRACKER_18, KINECT_DBG_TRACKER_19,
#endif
		};

		public enum AnalogId
		{
			GESTURE_LEFT_HAND_STATE = 0,
			GESTURE_LEFT_FOREHAND_VISIBILITY,
			GESTURE_RIGHT_HAND_STATE,
			GESTURE_RIGHT_FOREHAND_VISIBILITY,
			GESTURE_WIPSPEED,
			USER_HEIGHT,
#if CAVE_WITH_DEBUG_CHANNELS
			KINECT_DBG_ANALOG_0,  KINECT_DBG_ANALOG_1,   KINECT_DBG_ANALOG_2,
			KINECT_DBG_ANALOG_3,  KINECT_DBG_ANALOG_4,   KINECT_DBG_ANALOG_5,
			KINECT_DBG_ANALOG_6,  KINECT_DBG_ANALOG_7,   KINECT_DBG_ANALOG_8,
			KINECT_DBG_ANALOG_9,  KINECT_DBG_ANALOG_10,  KINECT_DBG_ANALOG_11,
			KINECT_DBG_ANALOG_12, KINECT_DBG_ANALOG_13,  KINECT_DBG_ANALOG_14,
			KINECT_DBG_ANALOG_15, KINECT_DBG_ANALOG_16,  KINECT_DBG_ANALOG_17,
			KINECT_DBG_ANALOG_18, KINECT_DBG_ANALOG_19,
#endif
		};

		public enum ButtonId
		{
#if CAVE_WITH_DEBUG_CHANNELS
			KINECT_DBG_BUTTON_0 = 0, KINECT_DBG_BUTTON_1,   KINECT_DBG_BUTTON_2,
			KINECT_DBG_BUTTON_3,     KINECT_DBG_BUTTON_4,   KINECT_DBG_BUTTON_5,
			KINECT_DBG_BUTTON_6,     KINECT_DBG_BUTTON_7,   KINECT_DBG_BUTTON_8,
			KINECT_DBG_BUTTON_9,     KINECT_DBG_BUTTON_10,  KINECT_DBG_BUTTON_11,
			KINECT_DBG_BUTTON_12,    KINECT_DBG_BUTTON_13,  KINECT_DBG_BUTTON_14,
			KINECT_DBG_BUTTON_15,    KINECT_DBG_BUTTON_16,  KINECT_DBG_BUTTON_17,
			KINECT_DBG_BUTTON_18,    KINECT_DBG_BUTTON_19,
#endif
		};
		#endregion

		#region Kinect DLLImports
		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __construct([MarshalAs(UnmanagedType.LPStr)]string address);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __destruct(IntPtr self);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __update(IntPtr self);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __getTracker(IntPtr self, Kinect.TrackerId id);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern ButtonState __getButtonState(IntPtr self, Kinect.ButtonId id);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern float __getAnalogValue(IntPtr self, Kinect.AnalogId id);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __addTrackerObserver(IntPtr self, IntPtr observer);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void __addButtonObserver(IntPtr self, IntPtr observer);

		[DllImport("KinectClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __addAnalogObserver(IntPtr self, IntPtr observer);
		#endregion

		public IntPtr NativePointer { get; private set; }

		public Kinect(string address)
		{
			NativePointer = __construct(address);
		}

		public void Dispose()
		{
			__destruct(NativePointer);
		}

		#region Kinect Wrapper Methods
		public void update()
		{
			__update(NativePointer);
		}

		public float getAnalogValue(Kinect.AnalogId id)
		{
			return __getAnalogValue(NativePointer, id);
		}

		public Matrix44f getTracker(Kinect.TrackerId id)
		{
			return new Matrix44f(__getTracker(NativePointer, id));
		}

		public ButtonState getButtonState(Kinect.ButtonId id)
		{
			return __getButtonState(NativePointer, id);
		}

		public void addTrackerObserver(TrackerObserverConnector<Kinect.TrackerId> observer)
		{
			__addTrackerObserver(NativePointer, observer.NativePointer);
		}

		public void addButtonObserver(ButtonObserverConnector<Kinect.ButtonId> observer)
		{
			__addButtonObserver(NativePointer, observer.NativePointer);
		}

		public void addAnalogObserver(AnalogObserverConnector<Kinect.AnalogId, float> observer)
		{
			__addAnalogObserver(NativePointer, observer.NativePointer);
		}
		#endregion
	};
	#endregion

	#region Navigation Mediators
	public class WalkingInPlaceNavigationMediator : IDisposable
	{
		#region WIPNavMed DLLImports
		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __wip_construct(IntPtr kinect, IntPtr navigationAdapter);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __wip_destruct(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __wip_setFactor(IntPtr self, float factor);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern float __wip_getFactor(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __wip_setEnabled(IntPtr self, bool enabled);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool __wip_isEnabled(IntPtr self);
		#endregion

		private IntPtr __nativePointer;

		public WalkingInPlaceNavigationMediator(Kinect kinect, AvatarAdapterConnector navigationAdapter)
		{
			__nativePointer = __wip_construct(kinect.NativePointer, navigationAdapter.NativePointer);
		}

		public void Dispose()
		{
			__wip_destruct(__nativePointer);
		}

		public void setFactor(float factor)
		{
			__wip_setFactor(__nativePointer, factor);
		}

		public float getFactor()
		{
			return __wip_getFactor(__nativePointer);
		}

		public void setEnabled(bool enabled)
		{
			__wip_setEnabled(__nativePointer, enabled);
		}

		public bool isEnabled()
		{
			return __wip_isEnabled(__nativePointer);
		}
	}

	public class RedirectToFrontNavigationMediator : IDisposable
	{
		#region RTFNavMed DLLImports
		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __rtf_construct(IntPtr kinect, IntPtr navigationAdapter);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __rtf_destruct(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern float __rtf_getCoefficient(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern float __rtf_getTheta(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __rtf_update(IntPtr self, float timediff);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __rtf_setEnabled(IntPtr self, bool enabled);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool __rtf_isEnabled(IntPtr self);
		#endregion

		private IntPtr __nativePointer;

		public RedirectToFrontNavigationMediator(Kinect kinect, AvatarAdapterConnector navigationAdapter)
		{
			__nativePointer = __rtf_construct(kinect.NativePointer, navigationAdapter.NativePointer);
		}

		public void Dispose()
		{
			__rtf_destruct(__nativePointer);
		}

		public float getCoefficient()
		{
			return __rtf_getCoefficient(__nativePointer);
		}

		public float getTheta()
		{
			return __rtf_getTheta(__nativePointer);
		}

		public void update(float timediff)
		{
			__rtf_update(__nativePointer, timediff);
		}

		public void setEnabled(bool enabled)
		{
			__rtf_setEnabled(__nativePointer, enabled);
		}

		public bool isEnabled()
		{
			return __rtf_isEnabled(__nativePointer);
		}
	}

	public class SimpleWiiWalkNavigationMediator : IDisposable
	{
		#region SWWNavMed DLLImports
		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __sww_construct(IntPtr kinect, IntPtr navigationAdapter);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __sww_destruct(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __sww_setTargetWalkVelocity(IntPtr self, float targetWalkVelocity);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern float __sww_getTargetWalkVelocity(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __sww_setEnabled(IntPtr self, bool enabled);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool __sww_isEnabled(IntPtr self);
		#endregion

		private IntPtr __nativePointer;

		public SimpleWiiWalkNavigationMediator(WiiMote wii, AvatarAdapterConnector navigationAdapter)
		{
			__nativePointer = __sww_construct(wii.NativePointer, navigationAdapter.NativePointer);
		}

		public void Dispose()
		{
			__sww_destruct(__nativePointer);
		}

		public void setTargetWalkVelocity(float walkVelocity)
		{
			__sww_setTargetWalkVelocity(__nativePointer, walkVelocity);
		}

		public float getTargetWalkVelocity()
		{
			return __sww_getTargetWalkVelocity(__nativePointer);
		}

		public void setEnabled(bool enabled)
		{
			__sww_setEnabled(__nativePointer, enabled);
		}

		public bool isEnabled()
		{
			return __sww_isEnabled(__nativePointer);
		}
	}

	public class SimpleWiiTurnNavigationMediator : IDisposable
	{
		#region SWWNavMed DLLImports
		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr __swt_construct(IntPtr kinect, IntPtr navigationAdapter);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __swt_destruct(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __swt_setTargetTurnVelocity(IntPtr self, float targetTurnVelocity);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern float __swt_getTargetTurnVelocity(IntPtr self);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void __swt_setEnabled(IntPtr self, bool enabled);

		[DllImport("NavigationInterface.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool __swt_isEnabled(IntPtr self);
		#endregion

		private IntPtr __nativePointer;

		public SimpleWiiTurnNavigationMediator(WiiMote wii, AvatarAdapterConnector navigationAdapter)
		{
			__nativePointer = __swt_construct(wii.NativePointer, navigationAdapter.NativePointer);
		}

		public void Dispose()
		{
			__swt_destruct(__nativePointer);
		}

		public void setTargetTurnVelocity(float turnVelocity)
		{
			__swt_setTargetTurnVelocity(__nativePointer, turnVelocity);
		}

		public float getTargetTurnVelocity()
		{
			return __swt_getTargetTurnVelocity(__nativePointer);
		}

		public void setEnabled(bool enabled)
		{
			__swt_setEnabled(__nativePointer, enabled);
		}

		public bool isEnabled()
		{
			return __swt_isEnabled(__nativePointer);
		}
	}
	#endregion
}
