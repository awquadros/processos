using System;

namespace Awfq.Comuns.Portas.Adaptadores.Persistencia
{
    public interface IGeradorIdentificador {
        /// <sumary>
        /// Obtém um novo Id para ser usado na criação de um novo Agregado
        /// </sumary>
        /// <returns>
        /// Um novo identificador único
        /// </returns>
        Guid ObtemProximoId();
    }
}