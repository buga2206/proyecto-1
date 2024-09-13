namespace proyecto 
{
    public abstract class Poder 
    {
        public abstract void Aplicar(Moto moto);  
    }

    public class HiperVelocidad : Poder
    {
        private readonly int duracion;  

        public HiperVelocidad(int duracionSegundos = 5) 
        {
            duracion = duracionSegundos;  
        }

        public override void Aplicar(Moto moto) 
        {
            moto.ActivarHiperVelocidad(duracion);  // Aplica la hipervelocidad en la moto durante el tiempo especificado
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
            moto.ActivarEscudo(duracion);  // Llama al método de la moto para activar el escudo por la duración especificada
        }
    }
}