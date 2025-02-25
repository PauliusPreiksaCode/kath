import { LicenceLedgerEntry } from "../user";
import { Box, Button, Grid, Typography, useTheme } from "@mui/material";
import { licenceType } from "@/types/enums/licenceType";
import { paymentStatus } from "@/types/enums/paymentStatus";
import { useState } from "react";
import TransferLicenceDialog from "./TransferLicenceDialog";

export default function LicenceList({ licenceList }: { licenceList: LicenceLedgerEntry[] }) {
    const [openTransferDialog, setOpenTransferDialog] = useState(false);
    const [selectedLedgerEntry, setSelectedLedgerEntry] = useState<LicenceLedgerEntry | null>(null);
    const theme = useTheme();

    return (
        <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            gap: '1rem',
            padding: '0.5rem',
            maxHeight: '40vh',
            overflowY: 'auto',
            overflowX: 'hidden',
        }}>
            <Typography sx={{
                fontSize: '2rem',
                fontWeight: 'bold',
                color: theme.palette.primary.main,
                textAlign: 'center',
            }}>
                User payments and licences
            </Typography>
            <Grid container spacing={2} sx={{ maxHeight: '25vh', overflowY: 'auto' }}>
                {licenceList.map((ledgerEntry) => (
                    <Grid item xs={12} key={ledgerEntry.id}>
                        <Box sx={{
                            display: 'flex',
                            flexDirection: 'row',
                            flexWrap: 'wrap',
                            gap: '0.2rem',
                            padding: '0.7rem',
                            border: '1px solid',
                            borderColor: theme.palette.primary.main,
                            borderRadius: '10px',
                            backgroundColor: ledgerEntry.isActive ? theme.palette.background.paper : theme.palette.action.disabledBackground,
                            opacity: ledgerEntry.isActive ? 1 : 0.6,
                            alignItems: 'center',
                        }}>
                            <Typography variant="h6" sx={{ fontWeight: 'bold', color: theme.palette.primary.main, flex: '1 1 5%' }}>
                                {ledgerEntry.licence?.name}
                            </Typography>
                            <Typography sx={{ flex: '1 1 20%' }}>
                                <strong>Purchase Date: {ledgerEntry.purchaseDate.split("T")[0]}</strong> 
                            </Typography>
                            <Typography sx={{ flex: '1 1 10%' }}>
                                <strong>Is Active:</strong> {ledgerEntry.isActive ? "Yes" : "No"}
                            </Typography>
                            <Typography sx={{ flex: '1 1 15%' }}>
                                <strong>Payment Status:</strong> {paymentStatus[ledgerEntry.paymentStatus]}
                            </Typography>
                            <Typography sx={{ flex: '1 1 20%' }}>
                                <strong>Licence Type:</strong> {licenceType[ledgerEntry.licence?.type]}
                            </Typography>
                            <Button
                                variant="contained"
                                disabled={!ledgerEntry.isActive}
                                sx={{
                                    flex: '1 1 10%',
                                    backgroundColor: theme.palette.primary.main,
                                    color: theme.palette.primary.contrastText,
                                    }}
                                    onClick={() => {
                                        setSelectedLedgerEntry(ledgerEntry);
                                        setOpenTransferDialog(true);
                                    }}
                            >
                                Transfer licence
                            </Button>
                        </Box>
                    </Grid>
                ))}
            </Grid>
            <TransferLicenceDialog 
                open={openTransferDialog} 
                onClose={() => setOpenTransferDialog(false)} 
                licenceLedger={selectedLedgerEntry} 
            />
        </Box>
    );
}
