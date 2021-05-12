using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Ariadna.Engine
{
    public class OrangescaleEffect : ShaderEffect
    {
        /// <summary>
        /// Dependency property for Input
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty(nameof(Input), typeof(OrangescaleEffect), 0);

        /// <summary>
        /// Dependency property for FilterColor
        /// </summary>
        public static readonly DependencyProperty FilterColorProperty =
            DependencyProperty.Register(nameof(FilterColor), typeof(Color), typeof(OrangescaleEffect),
                new PropertyMetadata(Color.FromArgb(255, 255, 120, 0), PixelShaderConstantCallback(0)));

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrangescaleEffect()
        {
            this.PixelShader = this.CreatePixelShader();

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(FilterColorProperty);
        }

        private PixelShader CreatePixelShader()
        {
            var pixelShader = new PixelShader();

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                pixelShader.UriSource = new Uri("/Ariadna.Engine;component/Themes/Orangescale.ps", UriKind.Relative);
            }

            return pixelShader;
        }

        /// <summary>
        /// Impicit input
        /// </summary>
        public Brush Input
        {
            get => (Brush)this.GetValue(InputProperty);

            set => this.SetValue(InputProperty, value);
        }

        /// <summary>
        /// The color used to tint the input.
        /// </summary>
        public Color FilterColor
        {
            get => (Color)this.GetValue(FilterColorProperty);

            set => this.SetValue(FilterColorProperty, value);
        }
    }
}