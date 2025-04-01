import { Button, Typography, Card, CardContent, useTheme, CardActionArea } from "@mui/material";
import { Licence } from "../user";
import { licenceType } from "@/types/enums/licenceType";
import { useBuyLicence } from "@/hooks/user";
import { useContext } from "react";
import { PaymentContext } from "@/services/paymentProvider";

export default function LicenceCard(licence : Licence) {
    const theme = useTheme();
    const buyLicence = useBuyLicence();
    const paymentContext = useContext(PaymentContext);

    const textStyle = {
        mt: '0.5rem',
        fontSize: '1.3rem',
        fontWeight: 'bold',
        color: theme.palette.primary.main,
        display: 'flex',
        justifyContent: 'center',
    };

    return (
        <Card sx={{
                height: '250px',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between', 
                p: 2,
                ":hover": {
                    boxShadow: '0 0 10px 0px rgba(0,0,0,0.2)',
                },
                maxHeight: '250px',
                minHeight: '250px'
        }}>
            <CardContent>
                <Typography sx={textStyle}>{licence.name} License</Typography>
                <Typography sx={textStyle}>Price: {licence.price} EUR</Typography>
                <Typography sx={textStyle}>License Type: {licenceType[licence.type]}</Typography>
                <Typography sx={textStyle}>License Duration: {licence.duration} days</Typography>
            </CardContent>
            <CardActionArea sx={{ textAlign: 'center' }}>
                <Button
                    variant="contained"
                    sx={{
                        width: '100%',
                        py: 1.5,
                        fontSize: '1.1rem', 
                        fontWeight: 'bold',
                        backgroundColor: theme.palette.primary.main,
                        color: theme.palette.primary.contrastText,
                    }}
                    onClick={ async () => {
                        const data = {
                            "licenceId": licence.id,
                        }
                        const result = await buyLicence.mutateAsync(data);
                        paymentContext.setPaymentLedgerEntry(result.ledgerEntry);
                        paymentContext.setPaymentSessionId(result.sessionId);

                        window.open(result.redirectUrl, '_blank', 'noopener,noreferrer');
                    }}
                >
                    Buy license
                </Button>
            </CardActionArea>
        </Card>
    );
}