import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Start from "./pages/Start";

const App = () => {

  return (
    <Routes>
      <Route path="/" element={<Start />}>
      </Route>
    </Routes>
  )

}

export default App;
