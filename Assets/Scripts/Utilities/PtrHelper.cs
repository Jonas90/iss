using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;


/// <summary>
/// Stellt Helfer zum Umgang mit Pointern, speziell in Verbindung mit
/// unamanged C++-Code/DLLs bereit.
/// </summary>
public class PtrHelper
{
	/// <summary>
	/// Ist notwendig, da Unity-Mono keine Tupel hat (.NET 2.0)
	/// (siehe GetFunctionPointerForDelegate)
	/// </summary>
	public struct DelegateTuple
	{
		public Delegate a;
		public Delegate b;
		public DelegateTuple(Delegate a, Delegate b)
		{
			this.a = a;
			this.b = b;
		}
	}

	/// <summary>
	/// Backport von IntPtr.Add von .NET 4.0: 
	/// Addiert einen Offset zum Wert eines Zeigers.
	/// 
	/// http://msdn.microsoft.com/de-de/library/system.intptr.add.aspx
	/// 
	/// http://stackoverflow.com/questions/1866236/add-offset-to-intptr
	/// </summary>
	/// <param name="oldptr">pointer</param>
	/// <param name="size">offset</param>
	/// <returns>IntPtr, der um offset hinter pointer liegt</returns>
	public static IntPtr Add(IntPtr oldptr, int size)
	{
		return new IntPtr(oldptr.ToInt64() + size);
	}

	/// <summary>
	/// Version von Marshall.GetFunctionPointerForDelegate, welche auch
	/// typisierte Delegates unterstützt.
	/// 
	/// http://msdn.microsoft.com/en-us/library/at4fb09f.aspx
	/// 
	/// http://www.codeproject.com/Tips/441743/A-look-at-marshalling-delegates-in-NET
	/// </summary>
	/// <typeparam name="T">Typ des Delegates</typeparam>
	/// <param name="d">Delegate</param>
	/// <param name="binder">Objekt, an den das Ergebnis gebunden wird.
	/// Muss auf jeden Fall am Leben erhalten werden, um es vom Garbage
	/// Collector zu verschonen</param>
	/// <returns>Pointer auf den Delegate.</returns>
	public static IntPtr GetFunctionPointerForDelegate<T>(T d, out object binder)
		where T : class
	{
		var del = d as Delegate;
		IntPtr result;

		try
		{
			result = Marshal.GetFunctionPointerForDelegate(del);
			binder = del;
		}
		catch (ArgumentException) // generic type delegate
		{
			var delegateType = typeof(T);
			var method = delegateType.GetMethod("Invoke");
			var returnType = method.ReturnType;
			var paramTypes =
				method
				.GetParameters()
				.Select((x) => x.ParameterType)
				.ToArray();

			// builder a friendly name for our assembly, module, and proxy type
			var nameBuilder = new StringBuilder();
			nameBuilder.Append(delegateType.Name);
			foreach (var pType in paramTypes)
			{
				nameBuilder
					.Append("`")
					.Append(pType.Name);
			}
			var name = nameBuilder.ToString();

			// check if we've previously proxied this type before
			var proxyAssemblyExist =
				AppDomain
				.CurrentDomain
				.GetAssemblies()
				.FirstOrDefault((x) => x.GetName().Name == name);

			Type proxyType;
			if (proxyAssemblyExist == null)
			{
				/// create a proxy assembly
				var proxyAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
					new AssemblyName(name),
					AssemblyBuilderAccess.Run
				);
				var proxyModule = proxyAssembly.DefineDynamicModule(name);
				// begin creating the proxy type
				var proxyTypeBuilder = proxyModule.DefineType(name,
					TypeAttributes.AutoClass |
					TypeAttributes.AnsiClass |
					TypeAttributes.Sealed |
					TypeAttributes.Public,
					typeof(MulticastDelegate)
				);
				// implement the basic methods of a delegate as the compiler does
				var methodAttributes =
					MethodAttributes.Public |
					MethodAttributes.HideBySig |
					MethodAttributes.NewSlot |
					MethodAttributes.Virtual;
				proxyTypeBuilder
					.DefineConstructor(
						MethodAttributes.FamANDAssem |
						MethodAttributes.Family |
						MethodAttributes.HideBySig |
						MethodAttributes.RTSpecialName,
						CallingConventions.Standard,
						new Type[] { typeof(object), typeof(IntPtr) })
					.SetImplementationFlags(
						MethodImplAttributes.Runtime |
						MethodImplAttributes.Managed
					);
				proxyTypeBuilder
					.DefineMethod(
						"BeginInvoke",
						methodAttributes,
						typeof(IAsyncResult),
						paramTypes)
					.SetImplementationFlags(
						MethodImplAttributes.Runtime |
						MethodImplAttributes.Managed);
				proxyTypeBuilder
					.DefineMethod(
						"EndInvoke",
						methodAttributes,
						null,
						new Type[] { typeof(IAsyncResult) })
					.SetImplementationFlags(
						MethodImplAttributes.Runtime |
						MethodImplAttributes.Managed);
				proxyTypeBuilder
					.DefineMethod(
						"Invoke",
						methodAttributes,
						returnType,
						paramTypes)
					.SetImplementationFlags(
						MethodImplAttributes.Runtime |
						MethodImplAttributes.Managed);
				// create & wrap an instance of the proxy type
				proxyType = proxyTypeBuilder.CreateType();
			}
			else
			{
				// pull the type from an existing proxy assembly
				proxyType = proxyAssemblyExist.GetType(name);
			}
			// marshal and bind the proxy so the pointer doesn't become invalid
			var repProxy = Delegate.CreateDelegate(proxyType, del.Target, del.Method);
			result = Marshal.GetFunctionPointerForDelegate(repProxy);
			binder = new PtrHelper.DelegateTuple(del, repProxy);
		}
		return result;
	}
}
