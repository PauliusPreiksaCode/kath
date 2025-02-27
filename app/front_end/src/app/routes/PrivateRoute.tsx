import { UserContext } from "@/services/authProvider";
import { useContext } from "react";
import { Navigate, Outlet } from "react-router-dom";


interface PrivateRouteProps {
    allowedRoles: string[];
}

function PrivateRoute({ allowedRoles} : PrivateRouteProps) {

    const userContext = useContext(UserContext);
    
    const hasAccess = userContext.roles?.some((role : string) => allowedRoles.includes(role));

    return hasAccess ?  <Outlet /> : <Navigate to="/login" />;
}

export default PrivateRoute;