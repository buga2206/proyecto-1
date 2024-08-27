namespace proyecto
{
    public abstract class Item
    {
        public abstract void Aplicar(Moto moto);
    }

    public class CeldaDeCombustible : Item
    {
        public override void Aplicar(Moto moto)
        {
            moto.Combustible = Math.Min(moto.Combustible + 20, 100);
        }
    }

    public class CrecimientoEstela : Item
    {
        public override void Aplicar(Moto moto)
        {
            moto.TamañoEstela += 1;
        }
    }

    public class Bomba : Item
    {

        public override void Aplicar(Moto moto)
        {
            // Lógica para destruir la moto o aplicar el daño correspondiente
            moto.Combustible = 0; // Por ejemplo, se puede vaciar el combustible o destruirla
        }
    }
}
