import { createContext, useState } from 'react';

interface Props {
    children?: React.ReactNode;
  }

  interface PaymentContextProps {
    ledgerEntry: string;
    sessionId: string;
    setPaymentLedgerEntry: (ledgerEntry: string) => void;
    setPaymentSessionId: (sessionId: string) => void;
  }

export const PaymentContext = createContext<PaymentContextProps>({
    ledgerEntry: '',
    sessionId: '',
    setPaymentLedgerEntry: () => {},
    setPaymentSessionId: () => {},
});

const PaymentProvider : React.FC<Props> = ({ children }) => {

    const [ledgerEntry, setLedgerEntry] = useState(() => localStorage.getItem("ledgerEntry") || '');
    const [sessionId, setSessionId] = useState(() => localStorage.getItem("sessionId") || '');

    const setPaymentLedgerEntry = (ledgerEntry: string) => {
        setLedgerEntry(ledgerEntry);
        localStorage.setItem("ledgerEntry", ledgerEntry);
    }

    const setPaymentSessionId = (sessionId: string) => {
        setSessionId(sessionId);
        localStorage.setItem("sessionId", sessionId);
    }

    const PaymentContextValues : PaymentContextProps = {
        ledgerEntry,
        sessionId,
        setPaymentLedgerEntry,
        setPaymentSessionId,
    };

    return (
        <PaymentContext.Provider value ={PaymentContextValues}>
        {children}
        </PaymentContext.Provider>
    );
};

export default PaymentProvider;