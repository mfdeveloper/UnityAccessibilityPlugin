using System.Threading.Tasks;
using UnityEngine;

namespace UAP.ScriptableObjects
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that implements the <a href="https://refactoring.guru/design-patterns/strategy/csharp/example">Strategy pattern</a>
    /// in order to use any implementation or plugin of multi-language localization (e.g implement official <i>com.unity.localization</i> integration)
    /// </summary>
    public abstract class LocalizationStrategy : ScriptableObject
    {
        public virtual string Localize(ref string key, MonoBehaviour elementWithLocalize = null)
        {
            return string.Empty;
        }

        public virtual Task<string> LocalizeAsync(ref string key, MonoBehaviour elementWithLocalize = null)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
