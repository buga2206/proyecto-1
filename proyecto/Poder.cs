namespace proyecto
{
    public abstract class Poder
    {
        public abstract void Aplicar(Moto moto);
    }

    public class HiperVelocidad : Poder
    {
        public override void Aplicar(Moto moto)
        {
            moto.AplicarHiperVelocidad();
        }
    }

    public class Escudo : Poder
    {
        private readonly int duracion;

        public Escudo(int duracionSegundos = 5)
        {
            duracion = duracionSegundos;
        }

        public override void Aplicar(Moto moto)
        {
            moto.ActivarEscudo(duracion);
        }
    }
}
