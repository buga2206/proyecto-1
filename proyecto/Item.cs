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
            moto.IncrementarCombustible(20);
        }
    }

    public class CrecimientoEstela : Item
    {
        public override void Aplicar(Moto moto)
        {
            moto.TamañoEstela += 1;
            moto.ActualizarMaxTrailLength();
        }
    }

    public class Bomba : Item
    {
        public override void Aplicar(Moto moto)
        {
            moto.Combustible = 0;
        }
    }
}
