import { useGetLicences, useGetUserLicences } from "@/hooks/user";
import { useState, useEffect } from "react";
import { Box, CircularProgress, Grid, Typography, useTheme } from "@mui/material";
import LicenceCard from "./components/LicenceCard";
import LicenceList from "./components/LicenceList";

export interface Licence {
    id: number;
    name: string;
    price: number;
    type: number;
    duration: number;
}

export interface LicenceLedgerEntry {
    id: number;
    purchaseDate: string;
    isActive: boolean;
    paymentStatus: number;
    licence: Licence;
}

export default function User() {

    const [licences, setLicences] = useState<any>([]);
    const [userLicences, setUserLicences] = useState<any>([]);
    const getLicenses = useGetLicences();
    const getUserLicences = useGetUserLicences();
    const Theme = useTheme();

    useEffect(() => {
        setLicences(getLicenses.data || [])
        setUserLicences(getUserLicences.data || [])
    }, [getLicenses.data, getUserLicences.data])

    if(getLicenses.isLoading || getLicenses.isFetching || getUserLicences.isLoading || getUserLicences.isFetching) {
        return <CircularProgress/>;
    }

    return (
        <>
        <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            gap: '1rem',
            padding: '1rem',
        }}>
            <Typography sx={{ 
                mt: '0.5rem', 
                fontSize: '2.5rem', 
                fontWeight: 'bold', 
                color: Theme.palette.primary.main,
                display: 'flex',
                justifyContent: 'center',
            }}>
                Licences
            </Typography>
            <Grid container spacing={2}>
                {licences.map((licence: Licence) => (
                    <Grid item xs={12} sm={6} md={6} lg={6} key={licence.id}>
                        <LicenceCard key={licence.id} {...licence}/>
                    </Grid>
                ))}
            </Grid>
            <LicenceList 
                licenceList={userLicences}
            />
        </Box>
        </>
    )
}