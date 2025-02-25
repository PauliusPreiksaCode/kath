import { Box, Typography, useTheme } from "@mui/material";
import { Link } from "react-router-dom";


export default function PaymentSuccess() {
    const Theme = useTheme();

    return (
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
                 Payment successful!
             </Typography>
             <Link to='/' replace style={{
                  color: Theme.palette.primary.main,
                  fontWeight: 'bold',
                  fontSize: '0.875rem',
                  textDecoration: 'none',
                  display: 'flex',
                  justifyContent: 'center',
                  marginTop: '1rem',
            }}>
                  Click here to go back to the homepage
            </Link>
        </Box>
    )
}