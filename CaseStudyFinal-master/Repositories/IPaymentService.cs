﻿using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IPaymentService
    {
        List<Payment> GetAllPayments();
        Payment GetPaymentById(int id);
        int AddPayment(Payment payment);
        string UpdatePayment(Payment payment);
        string DeletePayment(int id);
    }
}
