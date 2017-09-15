using System;
using Conditions;
using Newtonsoft.Json;

namespace FaceRecognition.Domain.Images
{
    public class Effect
    {
        [JsonProperty]
        public EffectType Type { get; protected set; }
        [JsonProperty]
        public float Blur { get; protected set; }
        [JsonProperty]
        public float RotationAngle { get; protected set; }
        [JsonProperty]
        public double ElasticSigma { get; protected set; }
        [JsonProperty]
        public double ElasticAlpha { get; protected set; }
        [JsonProperty]
        public int ElasticKernelSize { get; protected set; }

        public static Effect ConvertToGrayEffect(Effect baseEffect = null)
        {
            var effect = GetBaseEffect(baseEffect);

            if (effect.Type.HasFlag(EffectType.ConvertedToGray))
            {
                throw new ArgumentException("Can't convert to gray an already converted to gray image");
            }
            effect.Type = effect.Type | EffectType.ConvertedToGray;
            return effect;
        }
        public static Effect CropAndResizeEffect(Effect baseEffect = null)
        {
            var effect = GetBaseEffect(baseEffect);

            effect.Type = effect.Type | EffectType.CroppedAndResized;
            return effect;
        }
        public static Effect BlurEffect(float blur, Effect baseEffect = null)
        {
            Condition.Requires(blur, nameof(blur)).IsGreaterThan(0);

            var effect = GetBaseEffect(baseEffect);

            if (effect.Type.HasFlag(EffectType.Blurred))
            {
                throw new ArgumentException("Can't blur an already blurred image");
            }
            effect.Type = effect.Type | EffectType.Blurred;
            effect.Blur = blur;
            return effect;
        }
        public static Effect RotationEffect(float angle, Effect baseEffect = null)
        {
            Condition.Requires(angle, nameof(angle)).IsNotEqualTo(0);

            var effect = GetBaseEffect(baseEffect);

            effect.Type = effect.Type | EffectType.Rotated;
            effect.RotationAngle += angle;
            return effect;
        }
        public static Effect FlipEffect(FlipMode flipMode, Effect baseEffect = null)
        {
            Condition.Requires(flipMode, nameof(flipMode)).IsNotEqualTo(FlipMode.None);

            var effect = GetBaseEffect(baseEffect);

            if (effect.Type.HasFlag(EffectType.FlippedVertical) || effect.Type.HasFlag(EffectType.FlippedBoth) || effect.Type.HasFlag(EffectType.FlippedHorizontal))
            {
                throw new ArgumentException("Can't flip an already flipped image");
            }
            switch (flipMode)
            {
                case FlipMode.Horizontal:
                    effect.Type = effect.Type | EffectType.FlippedHorizontal;
                    break;
                case FlipMode.Vertical:
                    effect.Type = effect.Type | EffectType.FlippedVertical;
                    break;
                case FlipMode.Both:
                    effect.Type = effect.Type | EffectType.FlippedBoth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flipMode), flipMode, null);
            }
            return effect;
        }
        public static Effect ElasticDeformationEffect(double sigma, double alpha, int kernelSize, Effect baseEffect = null)
        {
            Condition.Requires(sigma, nameof(sigma)).IsNotEqualTo(0);
            Condition.Requires(alpha, nameof(alpha)).IsNotEqualTo(0);
            Condition.Requires(kernelSize, nameof(kernelSize)).IsGreaterThan(0);

            var effect = GetBaseEffect(baseEffect);

            if (effect.Type.HasFlag(EffectType.ElasticDistorsionated))
            {
                throw new ArgumentException("Can't elastic distorsion an already elastic distorsioned image");
            }
            effect.Type = effect.Type | EffectType.ElasticDistorsionated;
            effect.ElasticSigma = sigma;
            effect.ElasticAlpha = alpha;
            effect.ElasticKernelSize = kernelSize;
            return effect;
        }

        private static Effect GetBaseEffect(Effect actualEffect)
        {
            if (actualEffect != null)
            {
                return new Effect
                {
                    Type = actualEffect.Type,
                    Blur = actualEffect.Blur,
                    RotationAngle = actualEffect.RotationAngle,
                    ElasticSigma = actualEffect.ElasticSigma,
                    ElasticAlpha = actualEffect.ElasticAlpha,
                    ElasticKernelSize = actualEffect.ElasticKernelSize,
                };
            }
            return new Effect();
        }
    }
}