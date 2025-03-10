import { GroupProps } from "../GroupList";
import { Button, Typography, Card, CardContent, useTheme, CardActionArea, Grid } from "@mui/material";
import Avatar from '@mui/material/Avatar';
import { useContext, useState } from "react";
import { UserContext } from "@/services/authProvider";
import { useNavigate } from "react-router-dom";
import { Paths } from "@/types";
import { OrganizationContext } from "@/services/organizationProvider";
import DeleteGroupCard from "./DeleteGroupCard";
import EditGroupCard from "./EditGroupCard";

export default function GroupCard(group : GroupProps) {

    const theme = useTheme();
    const userContext = useContext(UserContext);
    const organizationContext = useContext(OrganizationContext);
    const navigation = useNavigate();
    const [openEditGroup, setOpenEditGroup] = useState<boolean>(false);
    const [openDeleteGroup, setOpenDeleteGroup] = useState<boolean>(false);

    const textStyle = {
        mt: '0.5rem',
        fontSize: '1.3rem',
        fontWeight: 'bold',
        color: theme.palette.primary.main,
        display: 'flex',
        justifyContent: 'center',
    };

    const buttonStyle ={
        width: '100%',
        py: 1.5,
        fontSize: '1.1rem', 
        fontWeight: 'bold',
        backgroundColor: theme.palette.primary.main,
        color: theme.palette.primary.contrastText,
    };

    const initials = group?.name
        .split(' ')
        .map(word => word[0])
        .join('')
        .substring(0, 2)
        .toUpperCase();

        const fullOwner = userContext?.roles?.includes('OrganizationOwner') && userContext.userId === organizationContext.organizationOwner;

    return (
        <>
        <Card sx={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between', 
                height: 'auto',
                p: 2,
                ":hover": {
                    boxShadow: '0 0 10px 0px rgba(0,0,0,0.2)',
                },
                minHeight: fullOwner ? '20rem' : '15rem',
        }}>
            <CardContent>
                <Grid container spacing={2}>
                    <Grid item xs={6} sx={{ borderRight: '1px solid #e0e0e0' }}>
                        <Avatar 
                            sx={{ 
                                bgcolor: theme.palette.primary.main, 
                                width: '6rem', 
                                height: '6rem', 
                                display: 'flex', 
                                justifyContent: 'center', 
                                alignItems: 'center' ,
                                fontSize: '2.5rem',
                                margin: 'auto',
                            }}
                        >
                            {initials}
                        </Avatar>
                        <Typography sx={
                            {
                                mt: '0.5rem',
                                fontSize: '2rem',
                                fontWeight: 'bold',
                                color: theme.palette.primary.main,
                                display: 'flex',
                                justifyContent: 'center',
                            }
                        }>{group.name}</Typography>
                    </Grid>
                    <Grid item xs={6} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
                        <Typography sx={textStyle}>Description: {group.description}</Typography>
                        <Typography sx={textStyle}>Entries: {group.entriesCount}</Typography>
                        <Typography sx={textStyle}>Creation Date: {group.creationDate.split('T')[0]}</Typography>
                    </Grid>
                </Grid>
            </CardContent>
            <CardActionArea sx={{ textAlign: 'center', cursor: 'auto' }} disableRipple={true}>
                {fullOwner && 
                    <Grid container spacing={2} sx={{ mb: '1rem' }}>
                        <Grid item xs={6}>
                            <Button
                                variant="contained"
                                sx={buttonStyle}
                                onClick={() => setOpenEditGroup(true)}
                            >
                                Edit
                            </Button>
                        </Grid>
                        <Grid item xs={6}>
                            <Button
                                variant="contained"
                                sx={buttonStyle}
                                onClick={() => setOpenDeleteGroup(true)}
                            >
                                Delete
                            </Button>
                        </Grid>
                    </Grid>
                }
                <Button
                    variant="contained"
                    sx={buttonStyle}
                    onClick={() => {
                        organizationContext?.setGroupSessionId(group.id);
                        navigation(Paths.ENTRIES);
                    }}
                >
                    View Group
                </Button>
            </CardActionArea>
        </Card>
        <EditGroupCard 
            open={openEditGroup} 
            onClose={() => setOpenEditGroup(false)} 
            group={group}
        />
        <DeleteGroupCard
            open={openDeleteGroup} 
            onClose={() => setOpenDeleteGroup(false)} 
            group={group}
        />
        </>
    );
}