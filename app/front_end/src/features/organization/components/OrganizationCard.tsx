import { Organization } from "../OrganizationList";
import { Button, Typography, Card, CardContent, useTheme, CardActionArea, Grid } from "@mui/material";
import Avatar from '@mui/material/Avatar';
import { useContext, useState } from "react";
import EditOrganizationCard from "./EditOrganizationCard";
import DeleteOrganizationCard from "./DeleteOrganizationCard";
import AddMemberCard from "./AddMemberCard";
import RemoveMemberCard from "./RemoveMemberCard";
import { UserContext } from "@/services/authProvider";
import { OrganizationContext } from "@/services/organizationProvider";
import { useNavigate } from "react-router-dom";
import { Paths } from "@/types";

export default function OrganizationCard(organization : Organization) {

    const theme = useTheme();
    const userContext = useContext(UserContext);
    const navigation = useNavigate();
    const organizationContext = useContext(OrganizationContext);
    const [openEditOrganization, setOpenEditOrganization] = useState<boolean>(false);
    const [openDeleteOrganization, setOpenDeleteOrganization] = useState<boolean>(false);
    const [openAddMember, setOpenAddMember] = useState<boolean>(false);
    const [openRemoveMember, setOpenRemoveMember] = useState<boolean>(false);

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

    const initials = organization?.name
        .split(' ')
        .map(word => word[0])
        .join('')
        .substring(0, 2)
        .toUpperCase();

    const fullOwner = userContext?.roles?.includes('OrganizationOwner') && userContext.userId === organization.ownerId;

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
                        }>{organization.name}</Typography>
                    </Grid>
                    <Grid item xs={6} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
                        <Typography sx={textStyle}>Description: {organization.description}</Typography>
                        <Typography sx={textStyle}>Members: {organization.membersCount}</Typography>
                        <Typography sx={textStyle}>Groups: {organization.groups.length}</Typography>
                        <Typography sx={textStyle}>Creation Date: {organization.creationDate.split('T')[0]}</Typography>
                    </Grid>
                </Grid>
            </CardContent>
            <CardActionArea sx={{ textAlign: 'center', cursor: 'auto' }} disableRipple={true}>
                {fullOwner && 
                    <Grid container spacing={2} sx={{ mb: '1rem' }}>
                        <Grid item xs={2}>
                            <Button
                                variant="contained"
                                sx={buttonStyle}
                                onClick={() => setOpenEditOrganization(true)}
                            >
                                Edit
                            </Button>
                        </Grid>
                        <Grid item xs={2}>
                            <Button
                                variant="contained"
                                sx={buttonStyle}
                                onClick={() => setOpenDeleteOrganization(true)}
                            >
                                Delete
                            </Button>
                        </Grid>
                        <Grid item xs={3}>
                            <Button
                                variant="contained"
                                sx={buttonStyle}
                                onClick={() => setOpenAddMember(true)}
                            >
                                Add Member
                            </Button>
                            </Grid>
                        <Grid item xs={5}>
                            <Button
                                variant="contained"
                                sx={buttonStyle}
                                onClick={() => setOpenRemoveMember(true)}
                            >
                                Remove Member
                            </Button>
                        </Grid>
                    </Grid>
                }
                <Button
                    variant="contained"
                    sx={buttonStyle}
                    onClick={() => {
                        organizationContext?.setOrganizationSessionId(organization.id);
                        organizationContext?.setOrganizationSessionOwner(organization.ownerId);
                        navigation(Paths.GROUP);
                    }}
                >
                    View Organization
                </Button>
            </CardActionArea>
        </Card>
        {fullOwner && (
            <>
                <EditOrganizationCard 
                    open={openEditOrganization} 
                    onClose={() => setOpenEditOrganization(false)} 
                    organization={organization}
                />
                <DeleteOrganizationCard
                    open={openDeleteOrganization} 
                    onClose={() => setOpenDeleteOrganization(false)} 
                    organization={organization}
                />
                <AddMemberCard
                    open={openAddMember} 
                    onClose={() => setOpenAddMember(false)} 
                    organization={organization}
                />
                <RemoveMemberCard
                    open={openRemoveMember} 
                    onClose={() => setOpenRemoveMember(false)} 
                    organization={organization}
                />
            </>
        )}
        </>
    );
}