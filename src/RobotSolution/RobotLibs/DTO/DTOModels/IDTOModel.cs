using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO.DTOModels
{
    public interface IDTOModel<T> where T : IDTOModel<T>
    {
        EDTOType EDTOType { get; }
        byte[] ToBytes();
        static abstract T FromBytes(byte[] data);
    }
}
