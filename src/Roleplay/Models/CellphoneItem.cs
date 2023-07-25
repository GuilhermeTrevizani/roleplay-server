using System;
using System.Collections.Generic;

namespace Roleplay.Models
{
    public class CellphoneItem
    {
        public bool ModoAviao { get; set; }

        public List<CellphoneItemContact> Contatos { get; set; } = new();

        public List<CellphoneItemCall> Chamadas { get; set; } = new();

        public List<CellphoneItemMessage> Mensagens { get; set; } = new();
    }

    public class CellphoneItemContact
    {
        public CellphoneItemContact(uint numero, string nome)
        {
            Numero = numero;
            Nome = nome;
        }

        public uint Numero { get; set; }

        public string Nome { get; set; }
    }

    public class CellphoneItemCall
    {
        public uint Numero { get; set; }

        public DateTime DataInicio { get; set;} = DateTime.Now;

        public DateTime DataTermino { get; set;}

        public CellphoneCallType Tipo { get; set; } = CellphoneCallType.Perdida;

        public bool Origem { get; set; } = true;
    }
    
    public class CellphoneItemMessage
    {
        public uint Numero { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public string Mensagem { get; set; }

        public CellphoneMessageType Tipo { get; set; }
    }
}