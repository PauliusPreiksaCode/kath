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
    clearGroupSessionId: () => void;
    clearOrganizationSessionId: () => void;
}

export const OrganizationContext = createContext<OrganizationContextProps>({
    groupId: '',
    organizationId: '',
    organizationOwner: '',
    setGroupSessionId: () => {},
    setOrganizationSessionId: () => {},
    setOrganizationSessionOwner: () => {},
    clearGroupSessionId: () => {},
    clearOrganizationSessionId: () => {},
});

const OrganizationProvider = ({ children } : Props) => {

    const [groupId, setGroupId] = useState(() => localStorage.getItem("groupId") || '');
    const [organizationId, setOrganizationId] = useState(() => localStorage.getItem("organizationId") || '');
    const [organizationOwner, setOrganizationOwner] = useState(() => localStorage.getItem("organizationOwner") || '');

    const setGroupSessionId = (groupId: string) => {
        setGroupId(groupId);
        localStorage.setItem('groupId', groupId);
    };

    const setOrganizationSessionId = (organizationId: string) => {
        setOrganizationId(organizationId);
        localStorage.setItem('organizationId', organizationId);
    }

    const setOrganizationSessionOwner = (organizationOwner: string) => {
        setOrganizationOwner(organizationOwner);
        localStorage.setItem('organizationOwner', organizationOwner);
    };

    const clearGroupSessionId = () => {
        setGroupId('');
        localStorage.removeItem('groupId');
    };

    const clearOrganizationSessionId = () => {
        setOrganizationId('');
        localStorage.removeItem('organizationId');
    };

    const OrganizationContextValues = {
        groupId,
        organizationId,
        organizationOwner,
        setGroupSessionId,
        setOrganizationSessionId,
        setOrganizationSessionOwner,
        clearGroupSessionId,
        clearOrganizationSessionId,
    };

    return (
        <OrganizationContext.Provider value ={OrganizationContextValues}>
        {children}
        </OrganizationContext.Provider>
    );
};

export default OrganizationProvider;