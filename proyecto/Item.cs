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
            moto.IncrementarCombustible(20);  // Incrementa el combustible
        }
    }

    public class CrecimientoEstela : Item 
    {
        public override void Aplicar(Moto moto)  
        {
            moto.TamañoEstela += 1;  // Incrementa el tamaño de la estela de la moto en 1
            moto.ActualizarMaxTrailLength();  // Llama al método para actualizar la longitud máxima de la estela
        }
    }

    public class Bomba : Item  
    {
        public override void Aplicar(Moto moto) 
        {
            moto.Combustible = 0;  // Establece el combustible de la moto en 0
        }
    }
}