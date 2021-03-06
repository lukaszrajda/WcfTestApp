﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeoLib.Contracts;
using GeoLib.Data;
using System.Threading;
using System.ServiceModel;
using System.Windows.Forms;

namespace GeoLib.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, 
        ReleaseServiceInstanceOnTransactionComplete = false)]
    public class GeoManager : IGeoService
    {
        public GeoManager()
        {
        }

        public GeoManager(IZipCodeRepository zipCodeRepository)
            : this(zipCodeRepository, null)
        {
        }

        public GeoManager(IStateRepository stateRepository)
            : this(null, stateRepository)
        {
        }

        public GeoManager(IZipCodeRepository zipCodeRepository, IStateRepository stateRepository)
        {
            _ZipCodeRepository = zipCodeRepository;
            _StateRepository = stateRepository;
        }

        IZipCodeRepository _ZipCodeRepository = null;
        IStateRepository _StateRepository = null;


        public ZipCodeData GetZipInfo(string zip)
        {
            ZipCodeData zipCodeData = null;

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);
            if (zipCodeEntity != null)
            {
                zipCodeData = new ZipCodeData()
                {
                    City = zipCodeEntity.City,
                    State = zipCodeEntity.State.Abbreviation,
                    ZipCode = zipCodeEntity.Zip
                };
            }
            else
            {
                //throw new ApplicationException($"Zip code {zip} not found.");
                //throw new FaultException($"Zip code {zip} not found.");
                ApplicationException ex = new ApplicationException($"Zip code {zip} not found.");
                throw new FaultException<ApplicationException>(ex, "Just another message");
            }
            return zipCodeData;
        }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            List<string> stateData = new List<string>();

            IStateRepository stateRepository = _StateRepository ?? new StateRepository();

            IEnumerable<State> states = stateRepository.Get(primaryOnly);
            if (states != null)
            {
                foreach (State state in states)
                    stateData.Add(state.Abbreviation);
            }

            return stateData;
        }

        public IEnumerable<ZipCodeData> GetZips(string state)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            var zips = zipCodeRepository.GetByState(state);
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

            return zipCodeData;
        }

        public IEnumerable<ZipCodeData> GetZips(string zip, int range)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            ZipCode zipEntity = zipCodeRepository.GetByZip(zip);
            IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(zipEntity, range);
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

            return zipCodeData;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void UpdateZipCity(string zip, string city)
        {
            IZipCodeRepository repository = _ZipCodeRepository ?? new ZipCodeRepository();
            ZipCode zipEntity = repository.GetByZip(zip);
            if (zipEntity != null)
            {
                zipEntity.City = city;
                repository.Update(zipEntity);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = false)]
        public void UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
        {
            IZipCodeRepository repository = _ZipCodeRepository ?? new ZipCodeRepository();

            //Dictionary<string, string> cityBatch = new Dictionary<string, string>();
            //foreach (var zipCity in zipCityData)
            //{
            //    cityBatch.Add(zipCity.ZipCode, zipCity.City);
            //}
            //repository.UpdateBatch(cityBatch);

            int counter = 0;
            foreach (var zipCityItem in zipCityData)
            {
                counter++;
                if (counter == 2)
                {
                    throw new FaultException("Sorry you cannot do that.");
                }
                ZipCode zipEntity = repository.GetByZip(zipCityItem.ZipCode);
                if (zipEntity != null)
                {
                    zipEntity.City = zipCityItem.City;
                    repository.Update(zipEntity);
                }
            }
            OperationContext.Current.SetTransactionComplete();
        }
    }
}
