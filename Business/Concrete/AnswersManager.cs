using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AnswersManager : IAnswersService
    {
        IAnswersDal _answersDal;

        public AnswersManager(IAnswersDal answersDal)
        {
            _answersDal = answersDal;
        }

        public IDataResult<List<Answers>> GetAnswersByProductId(int productId)
        {
            return new SuccessDataResult<List<Answers>>(_answersDal.GetAll(x => x.ProductId == productId));
        }
    }
}
