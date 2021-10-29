using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "soInstaller", menuName = "Installers/soInstaller")]
public class soInstaller : ScriptableObjectInstaller<soInstaller>
{
    public override void InstallBindings()
    {
    }
}