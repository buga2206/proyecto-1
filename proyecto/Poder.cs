namespace proyecto
{
    public abstract class Poder
    {
        public abstract void Aplicar(Moto moto);
    }

    public class Escudo : Poder
    {
        public override void Aplicar(Moto moto)
        {
            // Lógica para aplicar el escudo (si decides implementarlo más adelante)
        }
    }

    public class HiperVelocidad : Poder
    {
        public override void Aplicar(Moto moto)
        {
            // Lógica para aplicar la hiper velocidad (si decides implementarlo más adelante)
        }
    }

    // Otros poderes aquí...
}
