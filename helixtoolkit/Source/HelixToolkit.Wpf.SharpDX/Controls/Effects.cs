﻿namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;

    using global::SharpDX;
    using global::SharpDX.DXGI;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.D3DCompiler;
    using global::SharpDX.Direct3D;
    using System.Runtime.InteropServices;
    using System.IO;

    public sealed class RenderTechnique : IComparable
    {
        public RenderTechnique(string name)
        {
            this.Name = name;
        }

        public RenderTechnique(string name, Effect effect, InputLayout layout)
        {
            this.Name = name;
            this.Device = effect.Device;
            this.Effect = effect;
            this.EffectTechnique = effect.GetTechniqueByName(this.Name);
            this.InputLayout = layout;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Name.Equals(obj.ToString());
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj.ToString());
        }

        public static bool operator ==(RenderTechnique a, RenderTechnique b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Name.Equals(b.Name);
        }

        public static bool operator !=(RenderTechnique a, RenderTechnique b)
        {
            return !(a == b);
        }

        public string Name { get; private set; }

        public Effect Effect { get; private set; }

        public EffectTechnique EffectTechnique { get; private set; }

        public global::SharpDX.Direct3D11.Device Device { get; private set; }

        public InputLayout InputLayout { get; private set; }
    }


    public sealed class Techniques
    {
        static Techniques()
        {
            /// <summary>
            /// Names of techniques which are implemented by default
            /// </summary>
            RenderBlinn = new RenderTechnique("RenderBlinn");
            RenderPhong = new RenderTechnique("RenderPhong");

            RenderDiffuse = new RenderTechnique("RenderDiffuse");
            RenderColors = new RenderTechnique("RenderColors");
            RenderPositions = new RenderTechnique("RenderPositions");
            RenderNormals = new RenderTechnique("RenderNormals");
            RenderPerturbedNormals = new RenderTechnique("RenderPerturbedNormals");
            RenderTangents = new RenderTechnique("RenderTangents");
            RenderTexCoords = new RenderTechnique("RenderTexCoords");
            RenderWires = new RenderTechnique("RenderWires");

#if DEFERRED 
            RenderDeferred = new RenderTechnique("RenderDeferred");
            RenderGBuffer = new RenderTechnique("RenderGBuffer");
            RenderDeferredLighting = new RenderTechnique("RenderDeferredLighting");
            RenderScreenSpace = new RenderTechnique("RenderScreenSpace");
#endif

#if TESSELLATION 
            RenderPNTriangs = new RenderTechnique("RenderPNTriangs");
            RenderPNQuads = new RenderTechnique("RenderPNQuads");
#endif
            RenderCubeMap = new RenderTechnique("RenderCubeMap");
            RenderLines = new RenderTechnique("RenderLines");                        

            RenderTechniques = new List<RenderTechnique>
            { 
                RenderBlinn,
                RenderPhong, 

                RenderColors,
                RenderDiffuse,
                RenderPositions,
                RenderNormals,
                RenderPerturbedNormals,
                RenderTangents, 
                RenderTexCoords,
                RenderWires,
#if DEFERRED
                RenderDeferred,
                RenderGBuffer,  
#endif
                
#if TESSELLATION 
                RenderPNTriangs,
                RenderPNQuads,
#endif
            };

            TechniquesSourceDict = new Dictionary<RenderTechnique, byte[]>()
            {
                {     Techniques.RenderPhong,      Properties.Resources._default}, 
                {     Techniques.RenderBlinn,      Properties.Resources._default}, 
                {     Techniques.RenderCubeMap,    Properties.Resources._default}, 
                {     Techniques.RenderColors,     Properties.Resources._default}, 
                {     Techniques.RenderDiffuse,    Properties.Resources._default}, 
                {     Techniques.RenderPositions,  Properties.Resources._default}, 
                {     Techniques.RenderNormals,    Properties.Resources._default},
                {     Techniques.RenderPerturbedNormals,    Properties.Resources._default}, 
                {     Techniques.RenderTangents,   Properties.Resources._default}, 
                {     Techniques.RenderTexCoords,  Properties.Resources._default}, 
                {     Techniques.RenderWires,      Properties.Resources._default}, 
                {     Techniques.RenderLines,      Properties.Resources._default}, 
    #if TESSELLATION                                        
                {     Techniques.RenderPNTriangs,  Properties.Resources._default}, 
                {     Techniques.RenderPNQuads,    Properties.Resources._default}, 
    #endif 
    #if DEFERRED            
                {     Techniques.RenderDeferred,   Properties.Resources._deferred},
                {     Techniques.RenderGBuffer,    Properties.Resources._deferred},
                {     Techniques.RenderDeferredLighting , Properties.Resources._deferred},
    #endif
            };
        }

        internal static readonly Techniques Instance = new Techniques();

        internal static readonly Dictionary<RenderTechnique, byte[]> TechniquesSourceDict;

        private Techniques()
        {

        }

        /// <summary>
        /// Names of techniques which are implemented by default
        /// </summary>
        public static RenderTechnique RenderBlinn { get; private set; }// = new RenderTechnique("RenderBlinn");
        public static RenderTechnique RenderPhong { get; private set; }

        public static RenderTechnique RenderDiffuse { get; private set; }
        public static RenderTechnique RenderColors { get; private set; }
        public static RenderTechnique RenderPositions { get; private set; }
        public static RenderTechnique RenderNormals { get; private set; }
        public static RenderTechnique RenderPerturbedNormals { get; private set; }
        public static RenderTechnique RenderTangents { get; private set; }
        public static RenderTechnique RenderTexCoords { get; private set; }
        public static RenderTechnique RenderWires { get; private set; }
        public static RenderTechnique RenderCubeMap { get; private set; }
        public static RenderTechnique RenderLines { get; private set; }

#if TESSELLATION
        public static RenderTechnique RenderPNTriangs { get; private set; }
        public static RenderTechnique RenderPNQuads { get; private set; }
#endif

#if DEFERRED                 
        public static RenderTechnique RenderDeferred { get; private set; }
        public static RenderTechnique RenderGBuffer { get; private set; }
        public static RenderTechnique RenderDeferredLighting { get; private set; }
        public static RenderTechnique RenderScreenSpace { get; private set; }
#endif
        public static IEnumerable<RenderTechnique> RenderTechniques { get; private set; }
    }



    public sealed class EffectsManager : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly EffectsManager Instance = new EffectsManager();


        /// <summary>
        /// 
        /// </summary>
        static EffectsManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private EffectsManager()
        {
#if DX11
            var adapter = GetBestAdapter();
            if (adapter == null)
            {
                System.Windows.MessageBox.Show("No DirectX 10 or higher adapter found, a software adapter will be used!", "Warning");
                this.device = new global::SharpDX.Direct3D11.Device(DriverType.Warp, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
            }
            else
            {
                this.device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
                //this.device = new Direct3D11.Device(Direct3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport, Direct3D.FeatureLevel.Level_11_0); 
            }
#else
            this.device = new Direct3D11.Device(Direct3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport, Direct3D.FeatureLevel.Level_10_1);                        
#endif
            this.InitEffects();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Adapter GetBestAdapter()
        {
            using (var f = new Factory())
            {
                Adapter bestAdapter = null;
                var bestLevel = global::SharpDX.Direct3D.FeatureLevel.Level_10_0;

                foreach (var item in f.Adapters)
                {
                    var level = global::SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(item);
                    if (bestAdapter == null || level > bestLevel)
                    {
                        bestAdapter = item;
                        bestLevel = level;
                    }
                }
                return bestAdapter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static global::SharpDX.Direct3D11.Device Device { get { return Instance.device; } }

        /// <summary>
        /// 
        /// </summary>
        private global::SharpDX.Direct3D11.Device device;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        ~EffectsManager()
        {
            this.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        private void InitEffects()
        {
            try
            {

                // ------------------------------------------------------------------------------------
#if TESSELLATION
                RegisterEffect(Properties.Resources.Tessellation,
#else                
                RegisterEffect(Properties.Resources.Default,                                    
#endif
 new[] 
                { 
                    // put here the techniques which you want to use with this effect
                    Techniques.RenderPhong, 
                    Techniques.RenderBlinn, 
                    Techniques.RenderCubeMap, 
                    Techniques.RenderColors, 
                    Techniques.RenderDiffuse,
                    Techniques.RenderPositions,
                    Techniques.RenderNormals,
                    Techniques.RenderPerturbedNormals,
                    Techniques.RenderTangents, 
                    Techniques.RenderTexCoords, 
                    Techniques.RenderWires, 
                    Techniques.RenderLines,                                        
#if TESSELLATION
                    Techniques.RenderPNTriangs,                     
                    Techniques.RenderPNQuads,
#endif
                    
                });

#if DEFERRED  
                // ------------------------------------------------------------------------------------                
                RegisterEffect(Properties.Resources.Deferred, //Properties.Resources.Deferred,
                new[] 
                { 
                    Techniques.RenderDeferred, 
                    Techniques.RenderGBuffer,
                    Techniques.RenderDeferredLighting 
                });
#endif

                // ##############################################################################################

                // ------------------------------------------------------------------------------------
                var defaultInputLayout = new InputLayout(device, GetEffect(Techniques.RenderPhong).GetTechniqueByName(Techniques.RenderPhong.Name).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                    new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),             
                    new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),             
                    new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),  
           
                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),                 
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                });

                // ------------------------------------------------------------------------------------
                var linesInputLayout = new InputLayout(device, GetEffect(Techniques.RenderLines).GetTechniqueByName(Techniques.RenderLines.Name).GetPassByIndex(0).Description.Signature, new[] 
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),

                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),                 
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                });

                // ------------------------------------------------------------------------------------
                var cubeMapInputLayout = new InputLayout(device, GetEffect(Techniques.RenderCubeMap).GetTechniqueByName(Techniques.RenderCubeMap.Name).GetPassByIndex(0).Description.Signature, new[] 
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),                                                             
                });

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    Techniques.RenderCubeMap, 
#if DEFERRED 
                    Techniques.RenderDeferredLighting
#endif
                }, cubeMapInputLayout);

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    Techniques.RenderLines 
                },
                linesInputLayout);

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    // put here techniques which use the vertex layout below
                    Techniques.RenderPhong, 
                    Techniques.RenderBlinn,
                                        
                    Techniques.RenderDiffuse,
                    Techniques.RenderPositions,
                    Techniques.RenderNormals,
                    Techniques.RenderPerturbedNormals,
                    Techniques.RenderTangents, 
                    Techniques.RenderTexCoords, 
                    Techniques.RenderColors, 
                    Techniques.RenderWires, 
#if DEFERRED 
                    Techniques.RenderDeferred,
                    Techniques.RenderGBuffer,
#endif
                }, defaultInputLayout);

#if TESSELLATION
                // ------------------------------------------------------------------------------------
                var tessellationInputLayout = new InputLayout(device, GetEffect(Techniques.RenderPNTriangs).GetTechniqueByName(Techniques.RenderPNTriangs.Name).GetPassByIndex(0).Description.Signature, new[] 
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                    new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),             
                    new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),             
                    new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),            
                });

                RegisterLayout(new[] 
                { 
                    // put here techniques which use the vertex layout below
                    Techniques.RenderPNTriangs,
                    Techniques.RenderPNQuads, 
                }, tessellationInputLayout);
#endif
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(string.Format("Error registering effect: {0}", ex.Message), "Error");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderEffectString"></param>
        /// <param name="techniqueName"></param>
        /// <param name="sFlags"></param>
        /// <param name="eFlags"></param>
        internal void RegisterEffect(string shaderEffectString, RenderTechnique techniqueName, ShaderFlags sFlags = ShaderFlags.None, EffectFlags eFlags = EffectFlags.None)
        {
            RegisterEffect(shaderEffectString, new[] { techniqueName }, sFlags, eFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderEffectString"></param>
        /// <param name="techniqueNames"></param>
        /// <param name="sFlags"></param>
        /// <param name="eFlags"></param>
        internal void RegisterEffect(string shaderEffectString, RenderTechnique[] techniqueNames, ShaderFlags sFlags = ShaderFlags.None, EffectFlags eFlags = EffectFlags.None)
        {
#if PRECOMPILED_SHADERS

            try
            {
                var shaderBytes = Techniques.TechniquesSourceDict[techniqueNames[0]];
                this.RegisterEffect(shaderBytes, techniqueNames);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error registering effect: {0}", ex.Message), "Error");
            }
#else

#if DEBUG
            sFlags |= ShaderFlags.Debug;
            eFlags |= EffectFlags.None;
#else
            sFlags |= ShaderFlags.OptimizationLevel3;
            eFlags |= EffectFlags.None;       
#endif

            var preposessMacros = new List<ShaderMacro>();

#if DEFERRED
    #if DEFERRED_MSAA
            preposessMacros.Add(new ShaderMacro("DEFERRED_MSAA", true));
    #endif

    #if SSAO
            preposessMacros.Add(new ShaderMacro("SSAO", true));
    #endif
#endif
            var preprocess = ShaderBytecode.Preprocess(shaderEffectString, preposessMacros.ToArray(), new IncludeHandler());
			var hashCode = preprocess.GetHashCode();
            if (!File.Exists(hashCode.ToString()))
            {
                try
                {
                    var shaderBytes = ShaderBytecode.Compile(preprocess, "fx_5_0", sFlags, eFlags);
                    shaderBytes.Bytecode.Save(hashCode.ToString());
                    this.RegisterEffect(shaderBytes.Bytecode, techniqueNames);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(string.Format("Error compiling effect: {0}", ex.Message), "Error");
                }
            }
            else
            {
                var shaderBytes = ShaderBytecode.FromFile(hashCode.ToString());
                this.RegisterEffect(shaderBytes, techniqueNames);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderEffectBytecode"></param>
        /// <param name="techniques"></param>
        /// <param name="eFlags"></param>
        internal void RegisterEffect(byte[] shaderEffectBytecode, RenderTechnique[] techniques, EffectFlags eFlags = EffectFlags.None)
        {
            var effect = new Effect(device, shaderEffectBytecode, eFlags);
            foreach (var tech in techniques)
                data[tech.Name] = effect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="technique"></param>
        /// <param name="layout"></param>
        internal void RegisterLayout(RenderTechnique technique, InputLayout layout)
        {
            data[technique.Name + "Layout"] = layout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="techniques"></param>
        /// <param name="layout"></param>
        internal void RegisterLayout(RenderTechnique[] techniques, InputLayout layout)
        {
            foreach (var tech in techniques)
                data[tech.Name + "Layout"] = layout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        public Effect GetEffect(RenderTechnique technique)
        {
            return (Effect)data[technique.Name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        public InputLayout GetLayout(RenderTechnique technique)
        {
            return (InputLayout)data[technique.Name + "Layout"];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    var o = item.Value as IDisposable;
                    Disposer.RemoveAndDispose(ref o);
                }
            }
            this.data = null;

            Disposer.RemoveAndDispose(ref this.device);
            this.device = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private class IncludeHandler : Include, ICallbackable, IDisposable
        {
            public void Close(Stream stream)
            {
                stream.Close();
            }

            public Stream Open(IncludeType type, string fileName, Stream parentStream)
            {
                var codeString = Properties.Resources.ResourceManager.GetString(Path.GetFileNameWithoutExtension(fileName), System.Globalization.CultureInfo.InvariantCulture);

                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(codeString);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }

            public IDisposable Shadow
            {
                get
                {
                    return this.stream;
                }
                set
                {
                    if (this.stream != null)
                        this.stream.Dispose();
                    this.stream = value as Stream;
                }
            }

            public void Dispose()
            {
                stream.Dispose();
            }

            private Stream stream;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DefaultVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;

        public const int SizeInBytes = 4 * (4 + 4 + 2 + 3 + 3 + 3);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LinesVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public const int SizeInBytes = 4 * (4 + 4);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CubeVertex
    {
        public Vector4 Position;
        public const int SizeInBytes = 4 * 4;
    }
}
