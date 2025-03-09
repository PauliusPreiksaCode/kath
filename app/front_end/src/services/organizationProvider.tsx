import { createContext, useState } from 'react';

interface Props {
    children?: React.ReactNode;
}

interface OrganizationContextProps {
    groupId: string;
    organizationId: string;
    organizationOwner: string;
    setGroupSessionId: (groupId: string) => void;
    setOrganizationSessionId: (organizationId: string) => void;
    setOrganizationSessionOwner: (organizationOwner: string) => void;
}

export const OrganizationContext = createContext<OrganizationContextProps>({
    groupId: '',
    organizationId: '',
    organizationOwner: '',
    setGroupSessionId: () => {},
    setOrganizationSessionId: () => {},
    setOrganizationSessionOwner: () => {},
});

const OrganizationProvider = ({ children } : Props) => {

    const [groupId, setGroupId] = useState('');
    const [organizationId, setOrganizationId] = useState('');
    const [organizationOwner, setOrganizationOwner] = useState('');

    const setGroupSessionId = (groupId: string) => {
        setGroupId(groupId);
    };

    const setOrganizationSessionId = (organizationId: string) => {
        setOrganizationId(organizationId);
    }

    const setOrganizationSessionOwner = (organizationOwner: string) => {
        setOrganizationOwner(organizationOwner);
    };

    const OrganizationContextValues = {
        groupId,
        organizationId,
        organizationOwner,
        setGroupSessionId,
        setOrganizationSessionId,
        setOrganizationSessionOwner,
    };

    return (
        <OrganizationContext.Provider value ={OrganizationContextValues}>
        {children}
        </OrganizationContext.Provider>
    );
};

export default OrganizationProvider;