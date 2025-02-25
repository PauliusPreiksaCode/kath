import { useUpdatePayment } from "@/hooks/user";
import authService from "@/services/auth";
import { PaymentContext } from "@/services/paymentProvider";
import { useContext, useEffect } from "react";
import { useNavigate } from "react-router-dom";


export default function ProcessPayment() {
    const paymentContext = useContext(PaymentContext);
    const updatePayment = useUpdatePayment();
    const navigate = useNavigate();

    useEffect(() => {
        const processPayment = async () => {
            if (!paymentContext.sessionId || !paymentContext.ledgerEntry) {
                return;
            }

            try {
                const request = {
                    sessionId: paymentContext.sessionId,
                    ledgerId: paymentContext.ledgerEntry,
                };

                const result = await updatePayment.mutateAsync(request);

                if (result == "Payment successful") {
                    await authService.renewToken();
                    navigate("/payment-success");
                } else {
                    navigate("/payment-failed");
                }
            } catch (error) {
                navigate("/");
            }
        };

        processPayment();
    }, []);

    return <></>;
}