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
            moto.AplicarHiperVelocidad();  // Llama al método de la moto para aplicar el efecto de hiper velocidad
        }
    }

    public class Escudo : Poder 
    {
        private readonly int duracion;  // Campo para almacenar la duración del escudo en segundos

        public Escudo(int duracionSegundos = 5) 
        {
            duracion = duracionSegundos;  // Asigna la duración del escudo
        }

        public override void Aplicar(Moto moto)  
        {
            moto.ActivarEscudo(duracion);  // Llama al método de la moto para activar el escudo por la duración especificada
        }
    }
}