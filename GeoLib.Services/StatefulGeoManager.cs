using GeoLib.Contracts;
using GeoLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class StatefulGeoManager : IStatefulGeoService
    {
        ZipCode _zipCodeEntity = null;
        public ZipCodeData GetZipInfo()
        {
            ZipCodeData zipCodeData = null;
            if (_zipCodeEntity != null)
            {
                zipCodeData = new ZipCodeData
                {
                    City = _zipCodeEntity.City,
                    State = _zipCodeEntity.State.Abbreviation,
                    ZipCode = _zipCodeEntity.Zip
                };
            }
            return zipCodeData;
            
        }

        public IEnumerable<ZipCodeData> GetZips(int range)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            if (_zipCodeEntity != null)
            {
                IZipCodeRepository zipCodeRepository = new ZipCodeRepository();

                IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(_zipCodeEntity, range);
                if (zips != null)
                {
                    foreach (ZipCode zipCode in zips)
                    {
                        zipCodeData.Add(new ZipCodeData()
                        {
                            City = zipCode.City,
                            State = zipCode.State.Abbreviation,
                            ZipCode = zipCode.Zip
                        });
                    }
                }
            }
            return zipCodeData;
        }

        public void PushZip(string zip)
        {
            IZipCodeRepository repository = new ZipCodeRepository();
            _zipCodeEntity = repository.GetByZip(zip);
        }
    }
}
