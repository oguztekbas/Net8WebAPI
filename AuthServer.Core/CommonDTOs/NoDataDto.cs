using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.CommonDTOs
{
    // Response dönerken Response objesini kullanırken 200 status kodlu dönersek data yollamamıza gerek yok
    // onun için lazım veya başka herhangi bir durumda data dönmeyecek bir durumda lazım. Boş dönüyoruz yani.
    // Response class'ımızda data kısmında birşey dönmek zorunda olduğumuz için gerekiyor bu.
   
    public class NoDataDto
    {
    }
}
