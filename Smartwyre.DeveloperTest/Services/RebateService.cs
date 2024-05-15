using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;


    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore ?? throw new ArgumentNullException(nameof(rebateDataStore));
        _productDataStore = productDataStore ?? throw new ArgumentNullException(nameof(productDataStore));
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
            Product product = _productDataStore.GetProduct(request.ProductIdentifier);

            var result = new CalculateRebateResult();

            if (rebate == null || product == null)
            {
                result.Success = false;
            }
            else
            {
                switch (rebate.Incentive)
                {
                    case IncentiveType.FixedCashAmount:
                        result = CalculateFixedCashAmount(rebate, product);
                        break;

                    case IncentiveType.FixedRateRebate:
                        result = CalculateFixedRateRebate(rebate, product, request);
                        break;

                    case IncentiveType.AmountPerUom:
                        result = CalculateAmountPerUom(rebate, product, request);
                        break;
                }
            }

            if (result.Success)
            {
                StoreCalculationResult(rebate, result.RebateAmount);
            }

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region Private Methods

    private void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        var storeRebateDataStore = new RebateDataStore();
        storeRebateDataStore.StoreCalculationResult(rebate, rebateAmount);
    }


    private CalculateRebateResult CalculateFixedCashAmount(Rebate rebate, Product product)
    {
        var result = new CalculateRebateResult();
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount) || rebate.Amount == 0)
        {
            result.Success = false;
        }
        else
        {
            result.RebateAmount = rebate.Amount;
            result.Success = true;
        }

        return result;
    }

    private CalculateRebateResult CalculateFixedRateRebate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        var result = new CalculateRebateResult();
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) || rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
        {
            result.Success = false;
        }
        else
        {
            result.RebateAmount = product.Price * rebate.Percentage * request.Volume;
            result.Success = true;
        }

        return result;
    }

    private CalculateRebateResult CalculateAmountPerUom(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        var result = new CalculateRebateResult();
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) || rebate.Amount == 0 || request.Volume == 0)
        {
            result.Success = false;
        }
        else
        {
            result.RebateAmount = rebate.Amount * request.Volume;
            result.Success = true;
        }

        return result;
    }

    #endregion

}
