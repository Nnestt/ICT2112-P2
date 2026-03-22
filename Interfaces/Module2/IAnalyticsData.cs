namespace ProRental.Interfaces.Module2;

using ProRental.Domain.Entities;
using System;
using System.Collections.Generic;

public interface IAnalyticsData
{
    Analytic GetAnalytics(int targetID);
    List<Analytic> GetAnalyticsByDate(DateTime day);
    List<Analytic> GetAnalyticsByDate(DateTime start, DateTime end);
    List<Analytic> GetAnalyticsBySupplier(string supplier);
    List<Analytic> GetAnalyticsByProduct(string product);
}